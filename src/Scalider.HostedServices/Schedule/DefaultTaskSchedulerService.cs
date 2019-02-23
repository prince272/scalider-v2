using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Scalider.Hosting.Schedule.Triggers;

namespace Scalider.Hosting.Schedule
{

    /// <summary>
    /// Default implementation of the <see cref="ITaskSchedulerService"/> interface.
    /// </summary>
    [UsedImplicitly]
    [DebuggerDisplay("Scheduled = {_scheduledTasks.Count}")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class DefaultTaskSchedulerService : ITaskSchedulerService, IDisposable
    {

        private readonly List<ScheduledTask> _scheduledTasks = new List<ScheduledTask>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskSchedulerService"/> class.
        /// </summary>
        public DefaultTaskSchedulerService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTaskSchedulerService"/> class.
        /// </summary>
        /// <param name="scheduledTasks"></param>
        public DefaultTaskSchedulerService([NotNull] IEnumerable<ISchedulableTask> scheduledTasks)
        {
            Check.NotNull(scheduledTasks, nameof(scheduledTasks));

            _scheduledTasks.AddRange(
                scheduledTasks.Select(t => new ScheduledTask(t))
            );
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual void Schedule(ISchedulableTask schedulableTask)
        {
            Check.NotNull(schedulableTask, nameof(schedulableTask));

            _scheduledTasks.Add(new ScheduledTask(schedulableTask));
        }

        /// <inheritdoc />
        public IEnumerable<ScheduledTask> GetTasksPendingExecution(DateTimeOffset utcNow)
        {
            var scheduledTasksArray = _scheduledTasks.ToArray();
            foreach (var task in scheduledTasksArray)
            {
                var trigger = task.TaskInstance.Trigger ?? NullTrigger.Instance;
                if (!task.NextPossibleExecutionTimeUtc.HasValue)
                {
                    // Try to retrieve the next possible execution time
                    var dt = task.LastKnownExecutionTimeUtc ?? utcNow;
                    var nextExecutionTime = trigger.GetExecutionTimeAfter(dt, task.TotalExecutionCount);
                    if (nextExecutionTime == null)
                    {
                        // No possible execution time in range
                        if (trigger.ShouldRemoveTask)
                        {
                            // The trigger says we should remove the task from the pool
                            _scheduledTasks.Remove(task);
                        }

                        continue;
                    }

                    task.NextPossibleExecutionTimeUtc = nextExecutionTime;
                }

                // Determine if the task should be executed
                if (task.NextPossibleExecutionTimeUtc.Value > utcNow)
                {
                    // The task should not be executed at this time
                    continue;
                }

                task.ActualScheduledExecutionTimeUtc = task.NextPossibleExecutionTimeUtc.Value;
                task.NextPossibleExecutionTimeUtc = trigger.GetExecutionTimeAfter(
                    task.ActualScheduledExecutionTimeUtc,
                    task.TotalExecutionCount + 1
                );

                yield return task;
            }
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
            }

            _disposed = true;
        }

    }

}