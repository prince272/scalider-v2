using System;
using System.Threading.Tasks;

namespace Scalider.Hosting.Queue.Internal
{

    internal class CompiledExpressionQueueableTask<T> : IQueueableTask
        where T : class
    {

        private readonly Func<T, Func<QueuedTaskExecutionContext, Task>> _compiledExpression;

        public CompiledExpressionQueueableTask(Func<T, Func<QueuedTaskExecutionContext, Task>> compiledExpression)
        {
            _compiledExpression = compiledExpression;
        }

        #region IQueueableTask Members

        /// <inheritdoc />
        public Task RunAsync(QueuedTaskExecutionContext executionContext)
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