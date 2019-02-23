using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Template
{

    /// <summary>
    /// Represents an implementation of the <see cref="ITemplateRenderer"/> that doesn't alters the template
    /// output.
    /// </summary>
    [UsedImplicitly]
    public class NullTemplateRenderer : ITemplateRenderer
    {

        /// <summary>
        /// Provides a singleton instance of the <see cref="NullTemplateRenderer"/>.
        /// </summary>
        [UsedImplicitly]
        public static readonly ITemplateRenderer Instance = new NullTemplateRenderer();

        /// <inheritdoc />
        public Task<string> RenderAsync(string template, object model,
            CancellationToken cancellationToken = default) =>
            string.IsNullOrEmpty(template)
                ? Task.FromResult(string.Empty)
                : Task.FromResult(template);

    }

}