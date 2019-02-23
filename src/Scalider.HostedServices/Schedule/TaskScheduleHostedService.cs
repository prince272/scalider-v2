using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scalider.Hosting.Schedule
{

    /// <summary>
    /// Implementation of the <see cref="HostedServiceBase"/> class that uses <see cref="ITaskSchedulerService"/>
    /// to executed scheduled tasks.
    /// </summary>
    [UsedImplicitly]
    public class TaskScheduleHostedService : HostedServiceBase
    {

        private const int ExecutionDelay = 1 * 60_000;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskSchedulerService _taskSchedulerService;
        private readonly ITaskExceptionHandler _taskExceptionHandler;
        private readonly ILogger<TaskScheduleHostedService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskScheduleHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskSchedulerService"></param>
        /// <param name="logger"></param>
        public TaskScheduleHostedService(
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskSchedulerService taskSchedulerService,
            [NotNull] ILogger<TaskScheduleHostedService> logger)
            : this(serviceScopeFactory, taskSchedulerService, NullTaskExceptionHandler.Instance, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskScheduleHostedService"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="taskSchedulerService"></param>
        /// <param name="taskExceptionHandler"></param>
        /// <param name="logger"></param>
        public TaskScheduleHostedService(
            [NotNull] IServiceScopeFactory serviceScopeFactory,
            [NotNull] ITaskSchedulerService taskSchedulerService,
            [NotNull] ITaskExceptionHandler taskExceptionHandler,
            [NotNull] ILogger<TaskScheduleHostedService> logger)
        {
            Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            Check.NotNull(taskSchedulerService, nameof(taskSchedulerService));
            Check.NotNull(taskExceptionHandler, nameof(taskExceptionHandler));
            Check.NotNull(logger, nameof(logger));

            _serviceScopeFactory = serviceScopeFactory;
            _taskSchedulerService = taskSchedulerService;
            _taskExceptionHandler = taskExceptionHandler;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var executionTime = DateTimeOffset.UtcNow;
                var scheduledTasks = _taskSchedulerService.GetTasksPendingExecution(executionTime).ToArray();
                if (scheduledTasks.Length > 0)
                {
                    // We got at least one scheduled task, lets execute them all
                    var tasks = new List<Task>();
                    using (var serviceProvider = _serviceScopeFactory.CreateScope())
                    {
                        tasks.AddRange(scheduledTasks.Select(task =>
                            ExecuteScheduledTaskAsync(task, executionTime, serviceProvider.ServiceProvider,
                                cancellationToken)));

                        // Await all the tasks
                        await Task.WhenAll(tasks);
                    }
                }

                // We will wait a bit before trying again
                await Task.Delay(ExecutionDelay, cancellationToken);
            }
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private async Task ExecuteScheduledTaskAsync(ScheduledTask scheduledTask,
            DateTimeOffset executionTime, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            Exception exception = null;
            using (var ctx = new ScheduledTaskExecutionContext(serviceProvider, cancellationToken))
            {
                ctx.ExecutionCount = scheduledTask.IncrementAndGetExecutionCount();
                ctx.ExecutionTimeUtc = executionTime;
                ctx.ScheduledExecutionTimeUtc = scheduledTask.ActualScheduledExecutionTimeUtc;
                ctx.PreviousExecutionTimeUtc = scheduledTask.LastKnownExecutionTimeUtc;
                ctx.NextExecutionTimeUtc = scheduledTask.NextPossibleExecutionTimeUtc;

                scheduledTask.LastKnownExecutionTimeUtc = executionTime;

                // Really execute the task
                _logger.LogDebug($"Executing task \"{scheduledTask.Name}\"");
                var sw = Stopwatch.StartNew();
                try
                {
                    await scheduledTask.TaskInstance.RunAsync(ctx);
                }
                catch (Exception e)
                {
                    if (!(e is OperationCanceledException))
                        exception = e;
                }

                sw.Stop();
                _logger.LogDebug($"Executed task \"{scheduledTask.Name}\" in {sw.Elapsed.TotalMilliseconds}ms");
            }

            // Report to the exception handler
            TaskExecutionHelper.ReportUnhandledTaskException(
                exception,
                scheduledTask.TaskInstance,
                _taskExceptionHandler,
                serviceProvider,
                _logger
            );
        }

    }

}