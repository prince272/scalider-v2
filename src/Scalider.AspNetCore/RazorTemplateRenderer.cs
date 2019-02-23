using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scalider.Template;

namespace Scalider.AspNetCore
{

    /// <summary>
    /// Provides an implementation of the <see cref="ITemplateRenderer"/> interface that uses Razor to render
    /// templates using the view names as templates.
    /// </summary>
    [UsedImplicitly]
    public class RazorTemplateRenderer : ITemplateRenderer
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IViewEngine _viewEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="tempDataProvider"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public RazorTemplateRenderer([NotNull] IServiceProvider serviceProvider,
            [NotNull] ITempDataProvider tempDataProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(tempDataProvider, nameof(tempDataProvider));

            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;

            // Try to retrieve the view engine service
            _viewEngine = serviceProvider.GetService<ICompositeViewEngine>() ??
                          serviceProvider.GetService<IViewEngine>();

            if (_viewEngine == null)
            {
                // No view engine service registered on the service provider
                throw new InvalidOperationException(
                    $"No {typeof(ICompositeViewEngine).FullName} or {typeof(IViewEngine).FullName} available."
                );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="viewEngine"></param>
        /// <param name="tempDataProvider"></param>
        public RazorTemplateRenderer([NotNull] IServiceProvider serviceProvider,
            [NotNull] IViewEngine viewEngine, [NotNull] ITempDataProvider tempDataProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(viewEngine, nameof(viewEngine));
            Check.NotNull(tempDataProvider, nameof(tempDataProvider));

            _serviceProvider = serviceProvider;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="viewEngine"></param>
        /// <param name="tempDataProvider"></param>
        public RazorTemplateRenderer([NotNull] IServiceProvider serviceProvider,
            [NotNull] ICompositeViewEngine viewEngine, [NotNull] ITempDataProvider tempDataProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(viewEngine, nameof(viewEngine));
            Check.NotNull(tempDataProvider, nameof(tempDataProvider));

            _serviceProvider = serviceProvider;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        /// <inheritdoc />
        public async Task<string> RenderAsync(string template, object model,
            CancellationToken cancellationToken = default)
        {
            var actionContext = GetActionContext(cancellationToken);

            // Try to find the view with the given name
            var result = !string.IsNullOrEmpty(template) && IsApplicationRelativeViewName(template)
                ? _viewEngine.GetView(null, template, false)
                : _viewEngine.FindView(actionContext, template, false);

            // Determine if we were able to find a view with the given name
            if (!result.Success || result.View == null)
            {
                // Could not find the view using the current view engine
                throw new ArgumentException(
                    $"{template} does not match any available view",
                    nameof(template));
            }

            // Render the view
            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    result.View,
                    new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary()
                    ) {Model = model},
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                await result.View.RenderAsync(viewContext);
                return writer.ToString();
            }
        }

        private ActionContext GetActionContext(CancellationToken cancellationToken)
        {
            var actionContext = new ActionContext(
                new DefaultHttpContext
                {
                    RequestServices = _serviceProvider,
                    RequestAborted = cancellationToken
                },
                new RouteData(),
                new ActionDescriptor()
            );

            return actionContext;
        }

        private static bool IsApplicationRelativeViewName(string viewName) =>
            viewName.StartsWith("~") || viewName.StartsWith("/");

    }

}