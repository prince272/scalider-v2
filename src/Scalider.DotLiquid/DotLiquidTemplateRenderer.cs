using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotLiquid;
using DotLiquid.NamingConventions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Scalider.DotLiquid.Filters;
using Scalider.Template;

namespace Scalider.DotLiquid
{

    /// <summary>
    /// Provides an implementation of the <see cref="ITemplateRenderer"/> interface that uses the Liquid
    /// engine to render templates.
    /// </summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class DotLiquidTemplateRenderer : ITemplateRenderer
    {

        private static readonly ConcurrentDictionary<Type, MemberInfo[]> ModelMembersCache =
            new ConcurrentDictionary<Type, MemberInfo[]>();

        private readonly ILogger<DotLiquidTemplateRenderer> _logger;

        static DotLiquidTemplateRenderer()
        {
            global::DotLiquid.Template.NamingConvention = new RubyNamingConvention();

            // Custom filters
            global::DotLiquid.Template.RegisterFilter(typeof(MathFilters));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidTemplateRenderer"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public DotLiquidTemplateRenderer(ILogger<DotLiquidTemplateRenderer> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Converts an object to a <see cref="Hash"/>.
        /// </summary>
        /// <param name="model">The model to convert.</param>
        /// <returns>
        /// The converted model.
        /// </returns>
        protected virtual Hash ModelToHash([NotNull] object model)
        {
            Check.NotNull(model, nameof(model));

            return Hash.FromDictionary(ModelToDictionary(model));
        }

        #region ModelToDictionary

        private IDictionary<string, object> ModelToDictionary(object model)
        {
            var modelType = model.GetType();
            if (!ModelMembersCache.TryGetValue(modelType, out var modelMembers))
            {
                // The model type hasn't been cached before
                _logger.LogDebug("Trying to retrieve all the public fields and properties for {@ModelType}",
                    ReflectionUtils.GetTypeReadableName(modelType));

                var modelFields = modelType
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var modelProperties = modelType
                                      .GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                     BindingFlags.FlattenHierarchy)
                                      .Where(t => t.CanRead);

                modelMembers = modelFields.Concat(modelProperties.Cast<MemberInfo>()).ToArray();
                ModelMembersCache.TryAdd(modelType, modelMembers);
            }

            // Populate a dictionary using the properties
            var result = new Dictionary<string, object>();
            foreach (var mInfo in modelMembers)
            {
                _logger.LogDebug("Trying to resolve value for {@Member}",
                    ReflectionUtils.GetMemberDisplayName(mInfo));

                if (result.ContainsKey(mInfo.Name))
                {
                    // We already added a member with the same name to the result, this one will be skipped
                    _logger.LogDebug("Another member with the same name ({@MemberName}) has already been added",
                        mInfo.Name);

                    continue;
                }

                // Try to resolve the value for the field or property
                object memberValue = null;
                switch (mInfo)
                {
                    case FieldInfo fInfo:
                        memberValue = fInfo.GetValue(model);
                        break;
                    case PropertyInfo pInfo:
                        try
                        {
                            memberValue = pInfo.GetValue(model);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(
                                e,
                                "There was an unexpected exception while trying to retrieve the value for " +
                                "the property {@PropertyName} of type {@PropertyType}",
                                ReflectionUtils.GetMemberDisplayName(pInfo),
                                ReflectionUtils.GetTypeReadableName(pInfo.PropertyType)
                            );

                            // Add the default value for the property, that way it is still populated
                            result.Add(
                                pInfo.Name,
                                pInfo.PropertyType.IsValueType
                                    ? Activator.CreateInstance(pInfo.PropertyType)
                                    : null
                            );

                            continue;
                        }

                        break;
                }
                
                // Determine if we need to do any additional handling for the member value
                var valueType = memberValue?.GetType();
                if (memberValue == null || valueType.IsValueType)
                {
                    // We don't need to do any additional handling for the member value
                    result.Add(mInfo.Name, memberValue);
                    continue;
                }
                
                // Add the actual model value to the result
                result.Add(
                    mInfo.Name,
                    valueType != typeof(object)
                        ? ModelToDictionary(memberValue)
                        : memberValue
                );
            }

            // Done
            return result;
        }

        #endregion

        #region ITemplateRenderer Members

        /// <inheritdoc />
        public Task<string> RenderAsync(string template, object model = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return Task.FromResult(string.Empty);
            }

            // Parse the template
            _logger.LogDebug("Trying to parse the template {@Template}", template);
            var tpl = global::DotLiquid.Template.Parse(template);

            // Render the template
            string rendered;
            if (model == null)
            {
                // Render without model
                _logger.LogDebug("Trying to render template without local variables");
                rendered = tpl.Render();
            }
            else
            {
                // Render with a custom model
                var modelType = ReflectionUtils.GetTypeDisplayName(model.GetType());
                _logger.LogDebug(
                    "Trying to convert the model of type {@ModelType} to local variables for the template",
                    modelType
                );

                var localVariables = ModelToHash(model);
                if (localVariables == null)
                {
                    // We were unable to convert the model to local variables
                    _logger.LogDebug(
                        "The model of type {@ModelType} could not be converted to local variables, the " +
                        "template will be rendered without local variables",
                        modelType
                    );

                    rendered = tpl.Render();
                }
                else
                {
                    // The model was converted to local variables successfully
                    _logger.LogDebug(
                        "The model of type {@ModelType} was converted successfully to local " +
                        "variables {@LocalVariables}",
                        modelType,
                        localVariables
                    );

                    rendered = tpl.Render(localVariables);
                }
            }

            // Determine if there was any error while rendering the template
            if (tpl.Errors == null || tpl.Errors.Count == 0)
            {
                // The template was rendered without any error
                _logger.LogDebug("The template was rendered without any error");
                return Task.FromResult(rendered);
            }

            // There was one or more errors while trying to render the template
            _logger.LogDebug("The template was rendered with errors");
            throw new AggregateException(tpl.Errors);
        }

        #endregion

    }

}