using System.Reflection;
using System.Threading.Tasks;

namespace Scalider.Hosting.Schedule.Internal
{

    internal class StaticMemberAccessSchedulableTask : ISchedulableTask
    {

        private readonly MemberInfo _memberInfo;

        public StaticMemberAccessSchedulableTask(MemberInfo memberInfo, ITrigger trigger)
        {
            _memberInfo = memberInfo;
            Trigger = trigger;
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

        /// <inheritdoc />
        public ITrigger Trigger { get; }

        /// <inheritdoc />
        public Task RunAsync(ScheduledTaskExecutionContext executionContext)
        {
            var callback = TaskExecutionHelper.GetStaticMemberCallback<ScheduledTaskExecutionContext>(_memberInfo);
            if (callback == null)
                return Task.CompletedTask;

            return callback(executionContext) ?? Task.CompletedTask;
        }

    }

}