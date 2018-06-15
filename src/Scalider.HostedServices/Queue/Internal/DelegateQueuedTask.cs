using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue.Internal
{

    internal class DelegateQueueableTask : IQueueableTask
    {

        private readonly Func<IServiceProvider, CancellationToken, Task> _func;

        public DelegateQueueableTask([NotNull] Func<IServiceProvider, CancellationToken, Task> func)
        {
            _func = func;
        }

        #region IQueueableTask Members

        /// <inheritdoc />
        public Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            return _func(serviceProvider, cancellationToken) ?? Task.CompletedTask;
        }

        #endregion

    }

}