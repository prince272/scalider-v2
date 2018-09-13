using System;
using System.Threading.Tasks;

namespace Scalider.Hosting.Schedule.Internal
{

    internal class CompiledExpressionSchedulableTask<T> : ISchedulableTask
        where T : class
    {

        private readonly Func<T, Func<ScheduledTaskExecutionContext, Task>> _compiledExpression;

        public CompiledExpressionSchedulableTask(
            Func<T, Func<ScheduledTaskExecutionContext, Task>> compiledExpression,
            ITrigger trigger)
        {
            _compiledExpression = compiledExpression;
            Trigger = trigger;
        }

        #region ISchedulableTask Members

        /// <inheritdoc />
        public ITrigger Trigger { get; }

        /// <inheritdoc />
        public Task RunAsync(ScheduledTaskExecutionContext executionContext)
        {
            Check.NotNull(executionContext, nameof(executionContext));
            return TaskExecutionHelper.ExecuteExpressionCompilationOnInstance(
                executionContext.Services,
                _compiledExpression,
                executionContext
            );
        }

        #endregion

    }

}