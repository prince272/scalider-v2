using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Scalider.Hosting.Queue
{
    
    /// <summary>
    /// Implementation of the <see cref="HostedServiceBase"/> class that uses <see cref="ITaskQueueService"/>
    /// to execute the queued tasks.
    /// </summary>
    [UsedImplicitly]
    public class TaskQueueHostedService : HostedServiceBase
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskQueueService _taskQueueService;
        private readonly ITaskExceptionHandler _taskExceptionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueueHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskQueueService"></param>
        public TaskQueueHostedService([NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskQueueService taskQueueService)
            : this(serviceScopeFactory, taskQueueService, NullTaskExceptionHandler.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueueHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskQueueService"></param>
        /// <param name="taskExceptionHandler"></param>
        public TaskQueueHostedService([NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskQueueService taskQueueService, [NotNull] ITaskExceptionHandler taskExceptionHandler)
        {
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            Check.NotNull(taskQueueService, nameof(taskQueueService));
            Check.NotNull(taskExceptionHandler, nameof(taskExceptionHandler));
            
            _serviceScopeFactory = serviceScopeFactory;
            _taskQueueService = taskQueueService;
            _taskExceptionHandler = taskExceptionHandler;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var task = await _taskQueueService.DequeueAsync(cancellationToken);
                if (task == null)
                {
                    // For some reason, the service returned a null task
                    continue;
                }
                
                // Create a service scope for the task
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    // Execute the queued task
                    await ExecuteQueuedTaskAsync(task, serviceScope.ServiceProvider, cancellationToken);
                }
            }
        }

        #region ExecuteQueuedTaskAsync
        
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private async Task ExecuteQueuedTaskAsync(IQueueableTask queueableTask,
            IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        
            // Try to execute the task
            Exception exception = null;
            try
            {
                await queueableTask.RunAsync(serviceProvider, cancellationToken);
            }
            catch (Exception e)
            {
                if (!(e is OperationCanceledException))
                    exception = e;
            }
            finally
            {
                // Release the cancellation token source
                cts.Dispose();
            }
            
            // Report to the exception handler
            if (exception == null)
            {
                // There was no exception to report
                return;
            }
            
            var context = new UnhandledTaskExceptionContext(exception, serviceProvider);
            if (queueableTask is ITaskExceptionHandler queueableTaskExceptionHandler)
            {
                // The task implements an exception handler, we will let it handle the exception by itself
                queueableTaskExceptionHandler.OnUnhandledException(context);
            }

            // Determine if the task handled the exception
            if (!context.IsHandled)
            {
                // The task did not handle the exception, we will notify the global exception handler
                _taskExceptionHandler?.OnUnhandledException(context);
                if (!context.IsHandled)
                    throw exception;
            }
        }
        
        #endregion

    }
    
}