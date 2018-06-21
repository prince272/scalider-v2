using System;

namespace Scalider.Hosting.Schedule
{
    
    /// <summary>
    /// Defines the basic functionality of a scheduled task trigger.
    /// </summary>
    public interface ITrigger
    {

        /// <summary>
        /// Gets a value indicating whether the scheduled task should be removed from the task pool.
        /// </summary>
        bool ShouldRemoveTask { get; }
        
        /// <summary>
        /// Retrieves the next execution date and time (with timezone UTC) for a task after the given date
        /// and time. If no execution is possible, <c>null</c> should be returned.
        /// </summary>
        /// <param name="utcNow">The date and time (with timezone UTC) of the execution.</param>
        /// <param name="executionCount">The number of times task linked to this trigger has been executed.</param>
        /// <returns>
        /// The execution date and time (with timezone UTC) after the given date and time when the task
        /// should be exectued or <c>null</c> if no execution is possible after the given date and time.
        /// </returns>
        DateTimeOffset? GetExecutionTimeAfter(DateTimeOffset utcNow, int executionCount);

    }
    
}