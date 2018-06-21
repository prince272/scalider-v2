using System.Reflection;
using System.Threading.Tasks;

namespace Scalider.Hosting.Queue.Internal
{

    internal class StaticMemberAccessQueueableTask : IQueueableTask
    {

        private readonly MemberInfo _memberInfo;

        public StaticMemberAccessQueueableTask(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var mi = _memberInfo;
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
            var callback = TaskExecutionHelper.GetStaticMemberCallback<QueuedTaskExecutionContext>(_memberInfo);
            if (callback == null)
                return Task.CompletedTask;

            return callback(executionContext) ?? Task.CompletedTask;
        }

        #endregion

    }

}