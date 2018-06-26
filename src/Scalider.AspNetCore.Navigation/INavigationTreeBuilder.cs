using System.Threading;
using System.Threading.Tasks;

namespace Scalider.AspNetCore.Navigation
{
    
    /// <summary>
    /// Defines the basic functionality of a navigation tree builder.
    /// </summary>
    public interface INavigationTreeBuilder
    {

        /// <summary>
        /// Asynchronously creates a navigation tree.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task<NavigationTreeNode> BuildTreeAsync(CancellationToken cancellationToken = default);

    }
    
}