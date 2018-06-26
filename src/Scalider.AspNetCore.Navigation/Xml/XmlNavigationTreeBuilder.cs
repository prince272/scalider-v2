using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Scalider.AspNetCore.Navigation.Xml
{

    /// <summary>
    /// Represents an implementation of the <see cref="INavigationTreeBuilder"/> interface that uses an XML file
    /// to create a navigation tree.
    /// </summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class XmlNavigationTreeBuilder : INavigationTreeBuilder
    {

        private const string RootElementName = "node";
        private const string NodeElementName = "node";

        #region Node attributes

        private const string KeyAttributeName = "key";
        private const string EnabledAttributeName = "enabled";

        //
        private const string TitleAttributeName = "title";
        private const string LinkTargetAttributeName = "target";

        //
        private const string AreaAttributeName = "area";
        private const string ControllerAttributeName = "controller";
        private const string ActionAttributeName = "action";
        private const string RouteNameAttributeName = "route-name";
        private const string RouteNameAttributeAltName = "route-name";
        private const string RouteParamAttributePrefix = "route-";
        private const string UrlAttributeName = "url";

        //
        private const string RolesAttributeName = "roles";
        private const string AuthenticationPolicyNameAttributeName = "policy-name";
        private const string AuthenticationPolicyNameAttributeAltName = "policyName";
        private const string HideFromAttributeName = "hide-from";
        private const string HideFromAttributeAltName = "hideFrom";

        //
        private const string ClassNameAttributeName = "class-name";
        private const string ClassNameAttributeAltName = "className";
        private const string IconAttributeName = "icon";
        private const string IconClassNameAttributeName = "icon-class-name";
        private const string IconClassNameAttributeAltName = "iconClassName";

        #endregion

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly XmlNavigationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNavigationTreeBuilder"/> class.
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public XmlNavigationTreeBuilder([NotNull] IHostingEnvironment hostingEnvironment)
        {
            Check.NotNull(hostingEnvironment, nameof(hostingEnvironment));

            _hostingEnvironment = hostingEnvironment;
            _options = new XmlNavigationOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNavigationTreeBuilder"/> class.
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="options"></param>
        public XmlNavigationTreeBuilder([NotNull] IHostingEnvironment hostingEnvironment,
            [NotNull] IOptions<XmlNavigationOptions> options)
        {
            Check.NotNull(hostingEnvironment, nameof(hostingEnvironment));
            Check.NotNull(options, nameof(options));

            _hostingEnvironment = hostingEnvironment;
            _options = options.Value ?? new XmlNavigationOptions();
        }

        private static string GetAttributeValue(XElement element, string attributeName)
        {
            var attribute = element.Attribute(XName.Get(attributeName));
            return !string.IsNullOrWhiteSpace(attribute?.Value) ? attribute.Value : null;
        }

        private static bool GetAttributeBooleanValue(XElement element, string attributeName, bool defaultValue)
        {
            var value = GetAttributeValue(element, attributeName);
            return !string.IsNullOrWhiteSpace(value)
                ? bool.TryParse(value, out var attributeValue) && attributeValue
                : defaultValue;
        }

        #region CreateNode

        /// <summary>
        /// Creates a navigation node from the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The current XML element.</param>
        /// <param name="canHaveChildren">Whether the element can have children.</param>
        /// <returns>
        /// The new <see cref="NavigationTreeNode"/>.
        /// </returns>
        /// <exception cref="XmlNavigationException">When the node is invalid.</exception>
        protected virtual NavigationTreeNode CreateNode(XElement element, bool canHaveChildren)
        {
            var node = new NavigationNode
            {
                Key = GetAttributeValue(element, KeyAttributeName),
                Enabled = GetAttributeBooleanValue(element, EnabledAttributeName, true),

                Title = GetAttributeValue(element, TitleAttributeName),
                LinkTarget = GetAttributeValue(element, LinkTargetAttributeName),

                Area = GetAttributeValue(element, AreaAttributeName),
                Controller = GetAttributeValue(element, ControllerAttributeName),
                Action = GetAttributeValue(element, ActionAttributeName),
                RouteName = GetAttributeValue(element, RouteNameAttributeName) ??
                            GetAttributeValue(element, RouteNameAttributeAltName),
                RouteParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                Url = GetAttributeValue(element, UrlAttributeName),

                Roles = GetAttributeValue(element, RolesAttributeName) ?? "*",
                AuthorizationPolicyName = GetAttributeValue(element, AuthenticationPolicyNameAttributeName) ??
                                          GetAttributeValue(element, AuthenticationPolicyNameAttributeAltName),

                ClassName = GetAttributeValue(element, ClassNameAttributeName) ??
                            GetAttributeValue(element, ClassNameAttributeAltName),
                Icon = GetAttributeValue(element, IconAttributeName) ??
                       GetAttributeValue(element, IconClassNameAttributeName) ??
                       GetAttributeValue(element, IconClassNameAttributeAltName)
            };

            // Retrieve all the route parameters
            foreach (var attr in element.Attributes())
            {
                var attrName = attr.Name.LocalName ?? string.Empty;
                if (attrName.Length <= RouteParamAttributePrefix.Length ||
                    !attrName.StartsWith(RouteParamAttributePrefix) ||
                    string.Equals(RouteNameAttributeName, attrName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(RouteNameAttributeAltName, attrName, StringComparison.OrdinalIgnoreCase))
                {
                    // We are going to ignore the "route-name" attribute
                    continue;
                }

                var name = attrName.Substring(RouteParamAttributePrefix.Length);
                if (!string.IsNullOrEmpty(name))
                {
                    // We got a valid parameter name, we can continue
                    node.RouteParameters.TryAdd(name, attr.Value);
                }
            }
            
            // Retrieves a value indicating which kind of user the node should be hidden from
            var hideFrom = GetAttributeValue(element, HideFromAttributeName) ??
                           GetAttributeValue(element, HideFromAttributeAltName);

            if (!string.IsNullOrWhiteSpace(hideFrom))
            {
                // Determine if the hide-from value is valid
                if (!Enum.TryParse<HideNodeFrom>(hideFrom, true, out var result))
                {
                    // Unknown value
                    throw new XmlNavigationException(
                        $"The value '{hideFrom}' is not allowed for the '{HideFromAttributeName}' attribute."
                    );
                }

                node.HideNodeFrom = result;
            }

            // Determine if the node can have children
            var navigationNode = new NavigationTreeNode(node);
            if (!canHaveChildren)
            {
                // The node isn't allowed to have children, how sad
                return navigationNode;
            }

            // Retrieve all the children for the node
            var childElements = element.Elements(NodeElementName);
            foreach (var child in childElements)
            {
                var childNode = CreateNode(child, true);
                if (child == null)
                {
                    // The child node should be ignored
                    continue;
                }

                navigationNode.AddChild(childNode);
            }

            // Done
            return navigationNode;
        }

        #endregion

        #region INavigationTreeBuilder Members

        /// <inheritdoc />
        public virtual async Task<NavigationTreeNode> BuildTreeAsync(CancellationToken cancellationToken = default)
        {
            var filename = string.IsNullOrEmpty(_options.FileName)
                ? XmlNavigationOptions.DefaultFileName
                : _options.FileName;
            
            var file = Path.Combine(_hostingEnvironment.ContentRootPath, filename);
            if (!File.Exists(file))
            {
                // The file doesn't exists
                throw new XmlNavigationException(
                    "The system cannot find the file specified navigation definition file.",
                    new FileNotFoundException(null, file)
                );
            }

            // Read the XML content
            string xml;
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    xml = await reader.ReadToEndAsync();
                }
            }

            // Try to parse the document
            XDocument xDocument;
            try
            {
                xDocument = XDocument.Parse(xml);
            }
            catch (Exception e)
            {
                throw new XmlNavigationException(
                    "The content of the navigation definition file could not be parsed.",
                    e
                );
            }

            // Retrieve and validate the root element
            var rootElement = xDocument.Root;
            if (rootElement == null || !string.Equals(rootElement.Name.LocalName, RootElementName))
            {
                // Invalid root element
                throw new XmlNavigationException(
                    $"The root element must be a '{RootElementName}' but found " +
                    $"'{rootElement?.Name}' instead."
                );
            }

            // Try to create the tree root
            var tree = CreateNode(rootElement, false) ??
                       throw new XmlNavigationException($"The root node '{rootElement}' could not be parsed.");

            // Retrieve all the nodes
            var nodes = rootElement.Elements(XName.Get(NodeElementName));
            foreach (var node in nodes)
            {
                var treeNode = CreateNode(node, true);
                if (treeNode == null)
                {
                    // Failed to parse the node
                    throw new XmlNavigationException($"The node '{node}' could not be parsed.");
                }

                // Done with the node
                tree.AddChild(treeNode);
            }

            // Done
            return tree;
        }

        #endregion

    }

}