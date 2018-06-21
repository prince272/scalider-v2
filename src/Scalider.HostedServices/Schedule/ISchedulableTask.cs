using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Schedule
{
    
    /// <summary>
    /// Defines the basic functionality of a schedulable task.
    ///
    /// Due to the nature of scheduled tasks, a schedulable task should not depend on the execution of another
    /// schedulable task. This is because the service cannot guarantee that the tasks will execute in the
    /// desired order, neither that the tasks will be triggered one after another.
    /// </summary>
    public interface ISchedulableTask
    {
        
        /// <summary>
        /// Gets the <see cref="ITrigger"/> used for the task.
        /// </summary>
        ITrigger Trigger { get; }

        /// <summary>
        /// Called by <see cref="TaskScheduleHostedService"/> when the task schedule is triggered.
        /// </summary>
        /// <param name="executionContext">The execution context for the task.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task RunAsync([NotNull] ScheduledTaskExecutionContext executionContext);

    }
    
}