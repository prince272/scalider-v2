using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Template
{
    
    /// <summary>
    /// Defines the basic functionality of a template renderer.
    /// </summary>
    public interface ITemplateRenderer
    {

        /// <summary>
        /// Asynchronously renders the given <paramref name="template"/> and returns the generated string.
        /// </summary>
        /// <param name="template">The template to render.</param>
        /// <param name="model"></param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [UsedImplicitly]
        Task<string> RenderAsync(string template, object model, CancellationToken cancellationToken = default);

    }
    
}