using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Scalider.Hosting
{

    /// <summary>
    /// Represents an implementation of the <see cref="IHostedService"/> interface.
    /// </summary>
    public abstract class HostedServiceBase : IHostedService, IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;
        private Task _executingTask;
        private bool _disposed;

        /// <summary>
        /// Executes the hosted service asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether the dispose method was called.</param>
        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed && disposing)
            {
                // Dispose the resources allocated by the service
                _cancellationTokenSource?.Dispose();
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

        #region IHostedService Members

        /// <inheritdoc />
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                // The hosted service has been disposed
                throw new ObjectDisposedException(GetType().Name);
            }
            
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cancellationTokenSource.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            // Try to cancel the task being executed
            try
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                // Wait until the task being executed completes or the stop token is triggered
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

            // And then throw if the cancellation was requested
            cancellationToken.ThrowIfCancellationRequested();
        }

        #endregion

    }

}