using System;
using System.Diagnostics.CodeAnalysis;
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

namespace Scalider.AspNetCore
{
    
    /// <summary>
    /// Provides a base implementation for the <see cref="IViewRenderer"/>
    /// interface.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DefaultViewRenderer : IViewRenderer
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewRenderer"/>
        /// class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="viewEngine"></param>
        /// <param name="tempDataProvider"></param>
        public DefaultViewRenderer([NotNull] IServiceProvider serviceProvider,
            [NotNull] IViewEngine viewEngine,
            [NotNull] ITempDataProvider tempDataProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(viewEngine, nameof(viewEngine));
            Check.NotNull(tempDataProvider, nameof(tempDataProvider));

            _serviceProvider = serviceProvider;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        /// <inheritdoc />
        public virtual async Task<string> RenderViewToStringAsync(string viewName,
            object model, CancellationToken cancellationToken = default)
        {
            Check.NotNullOrEmpty(viewName, nameof(viewName));

            // Create the action context
            var actionContext = new ActionContext(
                new DefaultHttpContext
                {
                    RequestServices = _serviceProvider,
                    RequestAborted = cancellationToken
                },
                new RouteData(), new ActionDescriptor()
            );

            // Render the view
            using (var writer = new StringWriter())
            {
                var result = _viewEngine.FindView(actionContext, viewName, false);
                if (result.View == null)
                {
                    // Could not find the view using the current view engine
                    throw new ArgumentException(
                        $"{viewName} does not match any available view",
                        nameof(viewName));
                }

                // Create the view context
                var viewContext = new ViewContext(
                    actionContext,
                    result.View,
                    new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary()
                    ) {Model = model},
                    new TempDataDictionary(actionContext.HttpContext,
                        _tempDataProvider),
                    writer,
                    new HtmlHelperOptions()
                );

                // Render the view to the writer
                await result.View.RenderAsync(viewContext);

                // Done
                return writer.ToString();
            }
        }

    }
    
}