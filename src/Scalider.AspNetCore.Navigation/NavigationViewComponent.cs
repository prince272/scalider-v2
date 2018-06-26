using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Scalider.AspNetCore.Navigation
{

    /// <summary>
    /// Represents an implmentation of the <see cref="ViewComponent"/> class creates and displays the
    /// navigation tree.
    /// </summary>
    public class NavigationViewComponent : ViewComponent
    {

        private readonly INavigationTreeBuilder _navigationTreeBuilder;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewComponent"/> class.
        /// </summary>
        /// <param name="navigationTreeBuilder"></param>
        /// <param name="authorizationService"></param>
        /// <param name="loggerFactory"></param>
        public NavigationViewComponent([NotNull] INavigationTreeBuilder navigationTreeBuilder,
            [NotNull] IAuthorizationService authorizationService, [NotNull] ILoggerFactory loggerFactory)
        {
            Check.NotNull(navigationTreeBuilder, nameof(navigationTreeBuilder));
            Check.NotNull(authorizationService, nameof(authorizationService));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _navigationTreeBuilder = navigationTreeBuilder;
            _authorizationService = authorizationService;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Asynchronously creates and displays the navigation tree using the given <paramref name="viewName"/>.
        /// </summary>
        /// <param name="viewName">The name of the view used to display the navigation tree.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync([NotNull] string viewName)
        {
            Check.NotNullOrEmpty(viewName, nameof(viewName));
            var vm = new NavigationViewModel(
                await _navigationTreeBuilder.BuildTreeAsync(CancellationToken.None),
                HttpContext,
                Url,
                _authorizationService,
                _loggerFactory.CreateLogger<NavigationViewModel>()
            );

            return View(viewName, vm);
        }

    }

}