using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Scalider.Hosting.Queue.Internal
{

    internal class CompiledExpressionQueueableTask<T> : IQueueableTask
        where T : class
    {

        private readonly Func<T, Func<IServiceProvider, CancellationToken, Task>> _compiledExpression;

        public CompiledExpressionQueueableTask(Func<T, Func<IServiceProvider, CancellationToken, Task>> compiledExpression)
        {
            _compiledExpression = compiledExpression;
        }

        #region IQueueableTask Members

        /// <inheritdoc />
        public Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var instance = serviceProvider.GetService<T>();
            var isInstanceCreatedFromService = instance != null;
            if (instance == null)
            {
                // Create a new instance using the service provider
                instance = ActivatorUtilities.CreateInstance<T>(serviceProvider);
            }

            // Execute the task
            var task = _compiledExpression(instance)(serviceProvider, cancellationToken);
            if (!isInstanceCreatedFromService && instance is IDisposable disposable)
            {
                // We created a new instance and the instance is disposable, get rid of it after the task
                // execution completes
                task = task.ContinueWith((t, s) => ((IDisposable)s).Dispose(), disposable, cancellationToken);
            }
            
            // Done
            return task ?? Task.CompletedTask;
        }

        #endregion

    }

}