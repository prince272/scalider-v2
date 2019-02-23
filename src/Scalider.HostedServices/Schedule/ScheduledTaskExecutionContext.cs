using System;
using System.Threading;
using JetBrains.Annotations;

namespace Scalider.Hosting.Schedule
{

    /// <summary>
    /// Context for scheduled task execution.
    /// </summary>
    public class ScheduledTaskExecutionContext : IDisposable
    {

        private readonly CancellationTokenSource _cts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskExecutionContext"/> class.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cancellationToken"></param>
        public ScheduledTaskExecutionContext([NotNull] IServiceProvider services,
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

        /// <summary>
        /// Gets a value indicating the number of times the scheduled task has been executed.
        /// </summary>
        [UsedImplicitly]
        public int ExecutionCount { get; internal set; }

        /// <summary>
        /// Gets a value indicating the actual date and time (with timezone UTC)
        /// </summary>
        [UsedImplicitly]
        public DateTimeOffset ExecutionTimeUtc { get; internal set; }

        /// <summary>
        /// Gets a value indicating the date and time (with timezone UTC)
        /// </summary>
        [UsedImplicitly]
        public DateTimeOffset ScheduledExecutionTimeUtc { get; internal set; }

        /// <summary>
        /// Gets a value indicating the date and time (with timezone UTC) when the task last executed. The
        /// value will be <c>null</c> if this is the first time the task is being executed.
        /// </summary>
        [UsedImplicitly]
        public DateTimeOffset? PreviousExecutionTimeUtc { get; internal set; }

        /// <summary>
        /// Gets a value indicating the date and time (with timezone UTC) when the task next be executed. The
        /// value will be <c>null</c> if a next execution is not scheduled.
        /// </summary>
        [UsedImplicitly]
        public DateTimeOffset? NextExecutionTimeUtc { get; internal set; }

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