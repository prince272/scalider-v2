using System;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueueHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskQueueService"></param>
        public TaskQueueHostedService([NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskQueueService taskQueueService)
        {
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            Check.NotNull(taskQueueService, nameof(taskQueueService));
            
            _serviceScopeFactory = serviceScopeFactory;
            _taskQueueService = taskQueueService;
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
        
        private static async Task ExecuteQueuedTaskAsync(IQueueableTask queueableTask,
            IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        
            // Try to execute the task
            try
            {
                await queueableTask.RunAsync(serviceProvider, cancellationToken);
            }
            catch (Exception e)
            {
                if (!(e is OperationCanceledException))
                    throw;
            }
            finally
            {
                // Release the cancellation token source
                cts.Dispose();
            }
        }
        
        #endregion

    }
    
}