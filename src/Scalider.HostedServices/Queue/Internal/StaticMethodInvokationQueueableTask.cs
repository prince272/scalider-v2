using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Hosting.Queue.Internal
{

    internal class StaticMethodInvokationQueueableTask : IQueueableTask
    {

        private readonly MethodInfo _methodInfo;

        public StaticMethodInvokationQueueableTask(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var mi = _methodInfo;
            if (mi == null || mi.DeclaringType == null)
                return base.ToString();

            var declaringType = mi.DeclaringType;
            var assemblyName = declaringType.Assembly.GetName().Name;
            return $"{declaringType.FullName}.{mi.Name} ({assemblyName})";
        }

        #region IQueueableTask Members

        /// <inheritdoc />
        public Task RunAsync(QueuedTaskExecutionContext executionContext)
        {
            var task = _methodInfo.Invoke(null, new object[] {executionContext}) as Task;
            return task ?? Task.CompletedTask;
        }

        #endregion

    }

}