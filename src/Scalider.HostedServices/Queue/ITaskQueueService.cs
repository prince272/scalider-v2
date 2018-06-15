using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue
{
    
    public interface ITaskQueueService
    {

        /// <summary>
        /// Adds an <see cref="IQueueableTask"/> to the end of the queue.
        /// </summary>
        /// <param name="queueableTask">The <see cref="IQueueableTask"/> to add to the queue.</param>
        void Enqueue([NotNull] IQueueableTask queueableTask);

        /// <summary>
        /// Asynchornously adds an <see cref="IQueueableTask"/> to the end of the queue.
        /// </summary>
        /// <param name="queueableTask">The <see cref="IQueueableTask"/> to add to the queue.</param>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task EnqueueAsync([NotNull] IQueueableTask queueableTask, CancellationToken cancellationToken);
        
        /// <summary>
        /// Removes and returns the <see cref="IQueueableTask"/> at the beginning of the queue.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>
        /// The <see cref="IQueueableTask"/> that is removed from the beginning of the queue.
        /// </returns>
        Task<IQueueableTask> DequeueAsync(CancellationToken cancellationToken);

    }
    
}