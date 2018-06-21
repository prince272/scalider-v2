using System.Reflection;
using System.Threading.Tasks;

namespace Scalider.Hosting.Schedule.Internal
{

    internal class StaticMethodInvokationSchedulableTask : ISchedulableTask
    {

        private readonly MethodInfo _methodInfo;

        public StaticMethodInvokationSchedulableTask(MethodInfo methodInfo, ITrigger trigger)
        {
            _methodInfo = methodInfo;
            Trigger = trigger;
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

        #region ISchedulableTask Members

        /// <inheritdoc />
        public ITrigger Trigger { get; }

        /// <inheritdoc />
        public Task RunAsync(ScheduledTaskExecutionContext executionContext)
        {
            var task = _methodInfo.Invoke(null, new object[] {executionContext}) as Task;
            return task ?? Task.CompletedTask;
        }

        #endregion

    }

}