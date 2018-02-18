using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.AspNetCore
{

    /// <summary>
    /// Provides extension methods for the <see cref="IViewRenderer"/>
    /// interface.
    /// </summary>
    public static class ViewRendererExtensions
    {

        /// <summary>
        /// Renders the given <paramref name="viewName"/> and returns the
        /// generated string.
        /// </summary>
        /// <param name="renderer">The <see cref="IViewRenderer"/>.</param>
        /// <param name="viewName">The name of the view to render</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        public static Task<string> RenderViewToStringAsync(
            [NotNull] this IViewRenderer renderer, [NotNull] string viewName,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(renderer, nameof(renderer));
            Check.NotNullOrEmpty(viewName, nameof(viewName));

            return renderer.RenderViewToStringAsync(viewName, null,
                cancellationToken);
        }

    }
}