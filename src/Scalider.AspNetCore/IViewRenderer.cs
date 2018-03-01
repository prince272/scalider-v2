using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.AspNetCore
{
    
    /// <summary>
    /// Defines the basic functionality of a view renderer.
    /// </summary>
    public interface IViewRenderer
    {

        /// <summary>
        /// Renders the given <paramref name="viewName"/> and returns the
        /// generated string.
        /// </summary>
        /// <param name="viewName">The name of the view to render</param>
        /// <param name="model"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<string> RenderViewToStringAsync([NotNull, AspMvcView] string viewName, [CanBeNull] object model,
            CancellationToken cancellationToken = default);

    }
}