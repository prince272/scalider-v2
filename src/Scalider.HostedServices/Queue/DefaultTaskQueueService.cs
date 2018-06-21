using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue
{

    /// <summary>
    /// Default implementation of the <see cref="ITaskQueueService"/> interface that uses
    /// <see cref="ConcurrentQueue{T}"/> and a <see cref="SemaphoreSlim"/> to enqueue tasks.
    /// </summary>
    [UsedImplicitly]
    [DebuggerDisplay("Queued = {_semaphore.CurrentCount}")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class DefaultTaskQueueService : ITaskQueueService, IDisposable
    {

        private readonly ConcurrentQueue<IQueueableTask> _queuedTasks = new ConcurrentQueue<IQueueableTask>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskQueueService"/> class.
        /// </summary>
        public DefaultTaskQueueService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskQueueService"/> class.
        /// </summary>
        /// <param name="queuedTasks"></param>
        public DefaultTaskQueueService([NotNull] IEnumerable<IQueueableTask> queuedTasks)
        {
            Check.NotNull(queuedTasks, nameof(queuedTasks));

            // Determine if we received any queued tasks
            var queuedTasksArray = queuedTasks.Where(t => t != null).ToArray();
            if (queuedTasksArray.Length == 0)
            {
                // We didn't receive any queued tasks, no need to enqueue anything
                return;
            }

            // Enqueue all the received tasks
            foreach (var task in queuedTasksArray)
                _queuedTasks.Enqueue(task);

            _semaphore.Release(queuedTasksArray.Length);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether the dispose method was called.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Dispose the services allocated by the service
                _semaphore?.Dispose();
            }

            _disposed = true;
        }

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region ITaskQueueService Members

        /// <inheritdoc />
        public virtual void Enqueue(IQueueableTask queueableTask)
        {
            Check.NotNull(queueableTask, nameof(queueableTask));

            _queuedTasks.Enqueue(queueableTask);
            _semaphore.Release();
        }

        /// <inheritdoc />
        public virtual async Task<IQueueableTask> DequeueAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            _queuedTasks.TryDequeue(out var queuedTask);

            return queuedTask;
        }

        #endregion

    }

}