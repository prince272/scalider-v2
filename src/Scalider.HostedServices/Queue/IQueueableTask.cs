using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue
{
    
    /// <summary>
    /// Defines the basic functionality of a queueable task which is executed once by
    /// <see cref="TaskQueueHostedService"/> and then forgotten.
    /// </summary>
    public interface IQueueableTask
    {

        /// <summary>
        /// Callend by <see cref="TaskQueueHostedService"/> when the task is dequeued.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task RunAsync([NotNull] IServiceProvider serviceProvider, CancellationToken cancellationToken);

    }
    
}