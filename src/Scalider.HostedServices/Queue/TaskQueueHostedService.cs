using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<TaskQueueHostedService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueueHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskQueueService"></param>
        /// <param name="logger"></param>
        public TaskQueueHostedService(
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskQueueService taskQueueService,
            ILogger<TaskQueueHostedService> logger)
            : this(serviceScopeFactory, taskQueueService, NullTaskExceptionHandler.Instance, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueueHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskQueueService"></param>
        /// <param name="taskExceptionHandler"></param>
        /// <param name="logger"></param>
        public TaskQueueHostedService(
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskQueueService taskQueueService,
            [NotNull] ITaskExceptionHandler taskExceptionHandler,
            [NotNull] ILogger<TaskQueueHostedService> logger)
        {
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            Check.NotNull(taskQueueService, nameof(taskQueueService));
            Check.NotNull(taskExceptionHandler, nameof(taskExceptionHandler));
            Check.NotNull(logger, nameof(logger));
            
            _serviceScopeFactory = serviceScopeFactory;
            _taskQueueService = taskQueueService;
            _taskExceptionHandler = taskExceptionHandler;
            _logger = logger;
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
                    _logger.LogDebug("Got a NULL task, will ignore and wait for a valid task");
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
        private async Task ExecuteQueuedTaskAsync(IQueueableTask queuedTask,
            IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var taskName = TypeNameHelper.GetTypeDisplayName(queuedTask);

            // Try to execute the task
            Exception exception = null;
            using (var ctx = new QueuedTaskExecutionContext(serviceProvider, cancellationToken))
            {
                _logger.LogDebug($"Executing task \"{taskName}\"");
                var sw = Stopwatch.StartNew();
                try
                {
                    await queuedTask.RunAsync(ctx);
                }
                catch (Exception e)
                {
                    if (!(e is OperationCanceledException))
                        exception = e;
                }

                sw.Stop();
                _logger.LogDebug($"Executed task \"{taskName}\" in {sw.Elapsed.TotalMilliseconds}ms");
            }

            // Report to the exception handler

            // Report to the exception handler
            TaskExecutionHelper.ReportUnhandledTaskException(
                exception,
                queuedTask,
                _taskExceptionHandler,
                serviceProvider,
                _logger
            );
        }
        
        #endregion

    }
    
}