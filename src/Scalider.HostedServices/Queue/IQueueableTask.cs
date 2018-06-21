using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue
{
    
    /// <summary>
    /// Defines the basic functionality of a queueable task.
    /// </summary>
    public interface IQueueableTask
    {

        /// <summary>
        /// Callend by <see cref="TaskQueueHostedService"/> when the task is dequeued.
        /// </summary>
        /// <param name="executionContext">The execution context for the task.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task RunAsync([NotNull] QueuedTaskExecutionContext executionContext);

    }
    
}