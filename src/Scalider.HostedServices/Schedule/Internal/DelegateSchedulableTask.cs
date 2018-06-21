using System;
using System.Threading.Tasks;

namespace Scalider.Hosting.Schedule.Internal
{

    internal class DelegateSchedulableTask : ISchedulableTask
    {

        private readonly Func<ScheduledTaskExecutionContext, Task> _func;

        public DelegateSchedulableTask(Func<ScheduledTaskExecutionContext, Task> func, ITrigger trigger)
        {
            _func = func;
            Trigger = trigger;
        }

        #region ISchedulableTask Members

        /// <inheritdoc />
        public ITrigger Trigger { get; }

        /// <inheritdoc />
        public Task RunAsync(ScheduledTaskExecutionContext executionContext)
        {
            Check.NotNull(executionContext, nameof(executionContext));
            return _func(executionContext) ?? Task.CompletedTask;
        }

        #endregion

    }

}