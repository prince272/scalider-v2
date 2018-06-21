using System;
using JetBrains.Annotations;

namespace Scalider.Hosting.Schedule
{

    /// <summary>
    /// Encapsulates all information about an individual scheduled task.
    /// </summary>
    public class ScheduledTask
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTask"/> class.
        /// </summary>
        /// <param name="task"></param>
        public ScheduledTask([NotNull] ISchedulableTask task)
        {
            Check.NotNull(task, nameof(task));

            TaskInstance = task;
            Name = TaskExecutionHelper.GetTaskName(task);
        }

        /// <summary>
        /// Gets the instance of the <see cref="ISchedulableTask"/>.
        /// </summary>
        public ISchedulableTask TaskInstance { get; }

        /// <summary>
        /// Gets a textual representation of the task instance.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Gets a value indicating the total number of times the task has been executed.
        /// </summary>
        [UsedImplicitly]
        public int TotalExecutionCount { get; protected internal set; }
        
        /// <summary>
        /// Gets a value indicating the actual date and time (with timezone UTC) when the task was supposed to be
        /// executed.
        /// </summary>
        public DateTimeOffset ActualScheduledExecutionTimeUtc { get; protected internal set; }
        
        /// <summary>
        /// Gets a value indicating the last date and time (with timezone UTC) when the task was executed.
        /// </summary>
        public DateTimeOffset? LastKnownExecutionTimeUtc { get; protected internal set; }
        
        /// <summary>
        /// Gets a value indicating the next possible execution date and time (with timezone UTC) for the task.
        /// </summary>
        public DateTimeOffset? NextPossibleExecutionTimeUtc { get; protected internal set; }

        /// <summary>
        /// Increments and return the total number of execution for the task.
        /// </summary>
        /// <returns>
        /// The increased total number of executions.
        /// </returns>
        public int IncrementAndGetExecutionCount()
        {
            return ++TotalExecutionCount;
        }

    }

}