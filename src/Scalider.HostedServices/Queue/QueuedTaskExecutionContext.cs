using System;
using System.Threading;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue
{

    /// <summary>
    /// Context for queued task execution.
    /// </summary>
    public class QueuedTaskExecutionContext : IDisposable
    {

        private readonly CancellationTokenSource _cts;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedTaskExecutionContext"/> class.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cancellationToken"></param>
        public QueuedTaskExecutionContext([NotNull] IServiceProvider services,
            CancellationToken cancellationToken)
        {
            Check.NotNull(services, nameof(services));

            Services = services;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for the task.
        /// </summary>
        [UsedImplicitly]
        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> that the task has been aborted.
        /// </summary>
        [UsedImplicitly]
        public CancellationToken CancellationToken => _cts.Token;

        /// <inheritdoc />
        public void Dispose()
        {
            _cts?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cancels the execution of the task. The task is responsible of observing the token cancellation.
        /// </summary>
        public void Cancel()
        {
            if (!_cts.IsCancellationRequested)
            {
                // Issue a cancellation request for the token
                _cts.Cancel();
            }
        }

    }

}