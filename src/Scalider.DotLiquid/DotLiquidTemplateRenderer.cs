using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using DotLiquid.NamingConventions;
using JetBrains.Annotations;
using Microsoft.Extensions.Internal;
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

        private readonly IHashConverter _hashConverter;
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
        /// <param name="hashConverter"></param>
        /// <param name="logger"></param>
        public DotLiquidTemplateRenderer(IHashConverter hashConverter,
            ILogger<DotLiquidTemplateRenderer> logger)
        {
            _hashConverter = hashConverter;
            _logger = logger;
        }

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
                var modelType = TypeNameHelper.GetTypeDisplayName(model.GetType());
                _logger.LogDebug(
                    "Trying to convert the model of type {@ModelType} to local variables for the template",
                    modelType
                );

                var localVariables = _hashConverter.ToHash(model);
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