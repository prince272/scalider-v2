using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Scalider.AspNetCore.Navigation
{
    
    /// <summary>
    /// Provides a view model for the navigation component.
    /// </summary>
    public class NavigationViewModel
    {

        private readonly HttpContext _httpContext;
        private readonly IUrlHelper _urlHelper;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<NavigationViewModel> _logger;
        //
        private NavigationTreeNode _currentNode;
        private IEnumerable<NavigationTreeNode> _parentChain;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewModel"/> class.
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="httpContext"></param>
        /// <param name="urlHelper"></param>
        /// <param name="authorizationService"></param>
        /// <param name="logger"></param>
        public NavigationViewModel([NotNull] NavigationTreeNode rootNode, [NotNull] HttpContext httpContext,
            [NotNull] IUrlHelper urlHelper, [NotNull] IAuthorizationService authorizationService,
            [NotNull] ILogger<NavigationViewModel> logger)
        {
            Check.NotNull(rootNode, nameof(rootNode));
            Check.NotNull(httpContext, nameof(httpContext));
            Check.NotNull(urlHelper, nameof(urlHelper));
            Check.NotNull(authorizationService, nameof(authorizationService));
            Check.NotNull(logger, nameof(logger));

            RootNode = rootNode;
            _httpContext = httpContext;
            _urlHelper = urlHelper;
            _authorizationService = authorizationService;
            _logger = logger;
        }
        
        /// <summary>
        /// Gets the root node.
        /// </summary>
        public NavigationTreeNode RootNode { get; }

        /// <summary>
        /// Gets the node that matches the current request.
        /// </summary>
        public NavigationTreeNode CurrentNode => _currentNode ??
                                                 (_currentNode = RootNode?.Find(CurrentNodeMatcher));

        /// <summary>
        /// Gets the parent node of the current node.
        /// </summary>
        public NavigationTreeNode ParentNode => CurrentNode?.Parent;

        /// <summary>
        /// Gets the parent chain for the current node.
        /// </summary>
        [UsedImplicitly]
        public IEnumerable<NavigationTreeNode> ParentChain =>
            _parentChain ?? (_parentChain = GetParentChainForNode(CurrentNode));

        /// <summary>
        /// Creates a copy of the view model with a new root node.
        /// </summary>
        /// <param name="newRootNode">The new root node.</param>
        /// <returns>
        /// The view model with the new root node.
        /// </returns>
        public NavigationViewModel Copy([NotNull] NavigationTreeNode newRootNode)
        {
            Check.NotNull(newRootNode, nameof(newRootNode));
            if (newRootNode == RootNode)
                return this;

            return new NavigationViewModel(newRootNode, _httpContext, _urlHelper, _authorizationService, _logger)
            {
                _currentNode = CurrentNode,
                _parentChain = ParentChain
            };
        }

        #region ClassNames

        /// <summary>
        /// Retrieves a string containing the merged CSS class names for the given <paramref name="node"/> and
        /// <paramref name="classes"/>. 
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="classes">A dictionary containing a definition of which classes should be applied.</param>
        /// <returns>
        /// A string representing the CSS classes for the given <paramref name="node"/>.
        /// </returns>
        public string ClassNames([NotNull] NavigationTreeNode node, [NotNull] IDictionary<string, bool> classes)
        {
            Check.NotNull(node, nameof(node));
            Check.NotNull(classes, nameof(classes));
            
            var actualClasses = classes.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();
            return ClassNames(node, actualClasses);
        }

        /// <summary>
        /// Retrieves a string containing the merged CSS class names for the given <paramref name="node"/> and
        /// <paramref name="classes"/>. 
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="classes">The class names.</param>
        /// <returns>
        /// A string representing the CSS classes for the given <paramref name="node"/>.
        /// </returns>
        [UsedImplicitly]
        public string ClassNames([NotNull] NavigationTreeNode node, params string[] classes)
        {
            Check.NotNull(node, nameof(node));
            
            // Create a list containing a union of the classes defined by the node and the classes
            // given by the caller
            var classNames = new List<string>();
            if (!string.IsNullOrEmpty(node.Value.ClassName))
            {
                var nodeClasses = node.Value.ClassName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                classNames.AddRange(nodeClasses.Distinct());
            }

            if (classes != null && classes.Length > 0)
                classNames.AddRange(classes.Where(c => !string.IsNullOrEmpty(c)));
            
            // Done
            return string.Join(" ", classNames.Distinct());
        }
        
        #endregion

        #region ShouldDisplayAsync

        /// <summary>
        /// Asynchronously determines whether the given <paramref name="node"/> can be displayed based on
        /// the current request.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public async Task<bool> ShouldDisplayAsync([NotNull] NavigationTreeNode node,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(node, nameof(node));

            // Determine if the node should be hidden for the current user
            var hideFrom = node.Value.HideNodeFrom;
            if (hideFrom != HideNodeFrom.None)
            {
                if ((hideFrom & HideNodeFrom.All) == HideNodeFrom.All)
                {
                    // The node is hidden from all users, no other validation needed
                    return false;
                }

                // Determine if the node should be hidden for the current user
                var isAuthenticated = _httpContext.User?.Identity?.IsAuthenticated == true;
                if ((hideFrom & HideNodeFrom.Anonymous) == HideNodeFrom.Anonymous && !isAuthenticated)
                {
                    // The node should be hidden from anonymous users, and there is no authenticated user
                    return false;
                }

                if ((hideFrom & HideNodeFrom.Authenticated) == HideNodeFrom.Authenticated && isAuthenticated)
                {
                    // The node should be hidden from authenticated users, and there is an authenticated user
                    return false;
                }
            }

            // Determine if the node has an authorization policy
            if (!string.IsNullOrEmpty(node.Value.AuthorizationPolicyName))
            {
                // The node has an authorization policy
                try
                {
                    var authorizationResult = await _authorizationService
                        .AuthorizeAsync(_httpContext.User, node.Value.AuthorizationPolicyName);

                    return authorizationResult.Succeeded;
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "There was an unexpected exception while trying to authorize the navigation node " +
                        $"with key '{node.Value.Key}' which has an authorization policy named " +
                        $"'{node.Value.AuthorizationPolicyName}'");

                    return false;
                }
            }

            // The node doesn't have an authorization policy, lets try with the roles
            if (string.IsNullOrEmpty(node.Value.Roles) || node.Value.Roles == "*")
            {
                // The node doesn't specify any required role or is allowing all the users
                return true;
            }

            // Done
            var user = _httpContext.User;
            return user != null && user.IsInRole(node.Value.Roles);
        }

        #endregion

        /// <summary>
        /// Asynchronously determines whether the given <paramref name="node"/> has any visible children.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public async Task<bool> HasVisibleChildrenAsync([NotNull] NavigationTreeNode node,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(node, nameof(node));
            foreach (var child in node.Children)
            {
                if (await ShouldDisplayAsync(child, cancellationToken))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the given <paramref name="node"/> is the root node or is in the parent
        /// chain for the <paramref name="childNode"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="childNode">The child node.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="node"/> is the root node or is in the parent chain for
        /// <paramref name="childNode"/>.
        /// </returns>
        public bool IsParentOfNode([NotNull] NavigationTreeNode node, NavigationTreeNode childNode)
        {
            Check.NotNull(node, nameof(node));
            if (childNode == null)
                return false;
            
            return node.IsRootNode || GetParentChainForNode(childNode).Contains(node);
        }
        
        #region ResolveUrl

        /// <summary>
        /// Retrieves the link url for the given <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The link url.
        /// </returns>
        public string ResolveUrl([NotNull] NavigationTreeNode node)
        {
            var resolvedUrl = string.Empty;
            if (!string.IsNullOrEmpty(node.Value.Url))
            {
                // We got an actual URL for the node
                resolvedUrl = node.Value.Url;
                if (resolvedUrl.StartsWith("~"))
                {
                    // We got a tilde at the start of the URL, try to resolve it to a content
                    resolvedUrl = _urlHelper.Content(resolvedUrl);
                }
            }
            else if (!string.IsNullOrEmpty(node.Value.RouteName))
            {
                //
                resolvedUrl = _urlHelper.RouteUrl(node.Value.RouteName, GetRouteParameters(node));
            }
            else if (!string.IsNullOrEmpty(node.Value.Controller))
            {
                //
                var action = string.IsNullOrEmpty(node.Value.Action) ? "Index" : node.Value.Action;
                
                var parameters = GetRouteParameters(node);
                if (!string.IsNullOrEmpty(node.Value.Area))
                    parameters.TryAdd("area", node.Value.Area);

                resolvedUrl = _urlHelper.Action(action, node.Value.Controller, parameters);
            }

            // Done
            return resolvedUrl;
        }
        
        #endregion

        private static IDictionary<string, string> GetRouteParameters(NavigationTreeNode node)
        {
            var parameters = new Dictionary<string, string>();
            var routeParams = node.Value.RouteParameters ?? new Dictionary<string, string>();
            foreach (var param in routeParams)
            {
                parameters.TryAdd(param.Key, param.Value);
            }

            // Done
            return parameters;
        }

        private static IEnumerable<NavigationTreeNode> GetParentChainForNode(NavigationTreeNode node)
        {   
            if (node == null || node.IsRootNode)
            {
                // The node is the root node, we don't need to calculate anything
                return Array.Empty<NavigationTreeNode>();
            }
                
            // Calculate the parent chain until the root node
            var parentChain = new List<NavigationTreeNode>();
            var parent = node.Parent;
            
            while (parent != null)
            {
                parentChain.Add(parent);
                parent = parent.Parent;
            }
                
            // Done
            parentChain.Reverse();
            return parentChain.ToArray();
        }
        
        #region CurrentNodeMatcher

        private bool CurrentNodeMatcher(NavigationTreeNode node)
        {
            if (node == null)
                return false;
            
            //
            if (!string.IsNullOrEmpty(node.Value.Url))
            {
                //
                var url = node.Value.Url;
                if (url.StartsWith("~"))
                    url = _urlHelper.Content(url);
                
                return _httpContext.Request.Path.Equals(url);
            }
            
            //
            if (string.IsNullOrEmpty(node.Value.RouteName) && string.IsNullOrEmpty(node.Value.Controller))
            {
                //
                return false;
            }

            var routeData = _urlHelper.ActionContext.RouteData;
            var action = !string.IsNullOrEmpty(node.Value.RouteName)
                ? routeData.Values["action"] as string ?? "Index"
                : string.IsNullOrEmpty(node.Value.Action)
                    ? "Index"
                    : node.Value.Action;
            var controller = !string.IsNullOrEmpty(node.Value.RouteName)
                ? routeData.Values["controller"] as string ?? string.Empty
                : node.Value.Controller;

            if (!routeData.Values.TryGetValue("controller", out var currentController) ||
                !string.Equals(currentController as string, controller, StringComparison.OrdinalIgnoreCase) ||
                !routeData.Values.TryGetValue("action", out var currentAction) ||
                !string.Equals(currentAction as string, action, StringComparison.OrdinalIgnoreCase))
                return false;
            
            // Determine if the area matches
            if (!string.IsNullOrEmpty(node.Value.Area) &&
                (!routeData.Values.TryGetValue("area", out var currentArea) ||
                 !string.Equals(currentArea as string, node.Value.Area, StringComparison.OrdinalIgnoreCase)))
            {
                // The area doesn't match
                return false;
            }

            // The controller, action and area of the route matches the controller and action of the node,
            // determine if the other parameters match too
            foreach (var param in GetRouteParameters(node))
            {
                if (!routeData.Values.TryGetValue(param.Key, out var value) ||
                    value is string str && !string.Equals(str, param.Value, StringComparison.OrdinalIgnoreCase) ||
                    !Equals(value, param.Value))
                {
                    // The route parameter does not match the expected value
                    return false;
                }
            }
             
            // The parameters match
            return true;
        }
        
        #endregion
        
    }
    
}