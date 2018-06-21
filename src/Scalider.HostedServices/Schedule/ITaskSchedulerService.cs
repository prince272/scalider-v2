using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Scalider.Hosting.Schedule
{

    /// <summary>
    /// Defines the basic functionality of a task scheduler.
    /// </summary>
    public interface ITaskSchedulerService
    {

        /// <summary>
        /// Schedules a task for execution.
        /// </summary>
        /// <param name="schedulableTask">The <see cref="ISchedulableTask"/>.</param>
        void Schedule([NotNull] ISchedulableTask schedulableTask);

        /// <summary>
        /// Retrieves a collection of all the tasks that should be executed based on the given
        /// date and time.
        /// </summary>
        /// <param name="utcNow">The date and time (with timezone UTC) of the execution.</param>
        /// <returns>
        /// A collection containing all the tasks that should be executed.
        /// </returns>
        IEnumerable<ScheduledTask> GetTasksPendingExecution(DateTimeOffset utcNow);

    }

}