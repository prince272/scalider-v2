using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Hosting.Queue;
using Scalider.Hosting.Schedule;

namespace Scalider.Hosting.Queue.Internal
{

    internal class DelegateQueueableTask : IQueueableTask
    {

        private readonly Func<QueuedTaskExecutionContext, Task> _func;

        public DelegateQueueableTask(Func<QueuedTaskExecutionContext, Task> func)
        {
            _func = func;
        }

        #region IQueueableTask Members

        /// <inheritdoc />
        public Task RunAsync(QueuedTaskExecutionContext executionContext)
        {
            Check.NotNull(executionContext, nameof(executionContext));
            return _func(executionContext) ?? Task.CompletedTask;
        }

        #endregion

    }

}