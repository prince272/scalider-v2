using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Template
{

    /// <summary>
    /// Provides extension methods for the <see cref="ITemplateRenderer"/> interface.
    /// </summary>
    [UsedImplicitly]
    public static class TemplateRendererExtensions
    {

        /// <summary>
        /// Asynchronously renders the given <paramref name="template"/> and returns the generated string.
        /// </summary>
        /// <param name="renderer">The <see cref="ITemplateRenderer"/>.</param>
        /// <param name="template">The template to render.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [UsedImplicitly]
        public static Task<string> RenderAsync([NotNull] this ITemplateRenderer renderer,
            string template, CancellationToken cancellationToken = default)
        {
            Check.NotNull(renderer, nameof(renderer));
            return string.IsNullOrEmpty(template)
                ? Task.FromResult(string.Empty)
                : renderer.RenderAsync(template, null, cancellationToken);
        }

    }

}