using System.Threading;
using System.Threading.Tasks;

namespace Scalider.Template
{

    /// <summary>
    /// Represents an implementation of the <see cref="ITemplateRenderer"/> that doesn't alters the template
    /// output.
    /// </summary>
    public class NullTemplateRenderer : ITemplateRenderer
    {

        /// <summary>
        /// 
        /// </summary>
        public static readonly ITemplateRenderer Instance = new NullTemplateRenderer();

        #region ITemplateRenderer Members

        /// <inheritdoc />
        public Task<string> RenderAsync(string template, object model,
            CancellationToken cancellationToken = default) =>
            string.IsNullOrEmpty(template)
                ? Task.FromResult(string.Empty)
                : Task.FromResult(template);

        #endregion

    }

}