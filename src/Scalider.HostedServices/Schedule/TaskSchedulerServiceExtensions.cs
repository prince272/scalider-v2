using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Hosting.Schedule.Internal;

namespace Scalider.Hosting.Schedule
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="ITaskSchedulerService"/> interface.
    /// </summary>
    public static class TaskSchedulerServiceExtensions
    {

        /// <summary>
        /// Schedules the execution of a function delegate.
        /// </summary>
        /// <param name="schedulerService">The <see cref="ITaskSchedulerService"/>.</param>
        /// <param name="func">The function to add to the queue.</param>
        /// <param name="taskTrigger">The <see cref="ITrigger"/> for the task.</param>
        public static void Schedule([NotNull] this ITaskSchedulerService schedulerService,
            [NotNull] Func<Task> func, [NotNull] ITrigger taskTrigger)
        {
            Check.NotNull(schedulerService, nameof(schedulerService));
            Check.NotNull(func, nameof(func));
            Check.NotNull(taskTrigger, nameof(taskTrigger));

            schedulerService.Schedule(new DelegateSchedulableTask(ctx => func(), taskTrigger));
        }

        /// <summary>
        /// Schedules the execution of a function delegate.
        /// </summary>
        /// <param name="schedulerService">The <see cref="ITaskSchedulerService"/>.</param>
        /// <param name="func">The function to add to the queue.</param>
        /// <param name="taskTrigger">The <see cref="ITrigger"/> for the task.</param>
        public static void Schedule([NotNull] this ITaskSchedulerService schedulerService,
            [NotNull] Func<ScheduledTaskExecutionContext, Task> func, [NotNull] ITrigger taskTrigger)
        {
            Check.NotNull(schedulerService, nameof(schedulerService));
            Check.NotNull(func, nameof(func));
            Check.NotNull(taskTrigger, nameof(taskTrigger));

            schedulerService.Schedule(new DelegateSchedulableTask(func, taskTrigger));
        }
        
        #region Schedule<T>

        /// <summary>
        /// Schedules a task that queries or create a service of <typeparamref name="T"/> and executes the
        /// method or retrieves and executes the delegate using the member accessor.
        ///
        /// If <typeparamref name="T" /> is not registered as a service, then a new instance is created when the
        /// task is executed.
        /// </summary>
        /// <typeparam name="T">The type of the delegate that the <see cref="Expression{TDelegate}"/>
        /// represents.</typeparam>
        /// <param name="schedulerService">The <see cref="ITaskSchedulerService"/>.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="taskTrigger">The <see cref="ITrigger"/> for the task.</param>
        /// <exception cref="ArgumentException">When the <paramref name="expression"/> is invalid.</exception>
        public static void Schedule<T>([NotNull] this ITaskSchedulerService schedulerService,
            [NotNull] Expression<Func<T, Func<ScheduledTaskExecutionContext, Task>>> expression,
            [NotNull] ITrigger taskTrigger)
            where T : class
        {
            Check.NotNull(schedulerService, nameof(schedulerService));
            Check.NotNull(expression, nameof(expression));
            Check.NotNull(taskTrigger, nameof(taskTrigger));
            
            // Retrieve the real expression
            var exp = expression.Body;
            if (exp is UnaryExpression unaryExpression)
                exp = unaryExpression.Operand;
            
            // Determine whether the expression body is null
            if (exp is ConstantExpression constantExpression && constantExpression.Value  == null)
            {
                throw new ArgumentException(
                    "The body of the expression must be a method call or member access, instead got null.",
                    nameof(expression)
                );
            }
            
            // Enqueue the task
            switch (exp)
            {
                case MemberExpression memberExpression:
                    if (memberExpression.Member is PropertyInfo propertyInfo && propertyInfo.GetMethod != null &&
                        propertyInfo.GetMethod.IsStatic)
                    {
                        schedulerService.Schedule(
                            new StaticMemberAccessSchedulableTask(memberExpression.Member, taskTrigger));
                        return;
                    }

                    schedulerService.Schedule(
                        new CompiledExpressionSchedulableTask<T>(expression.Compile(), taskTrigger));
                    break;
                case MethodCallExpression methodCallExpression:
                    if (methodCallExpression.Object is ConstantExpression methodConstantExpression &&
                        methodConstantExpression.Value is MethodInfo methodInfo &&
                        methodInfo.IsStatic)
                    {
                        // The method call is to a static method, no need to compile the expression
                        schedulerService.Schedule(new StaticMethodInvokationSchedulableTask(methodInfo, taskTrigger));
                        return;
                    }

                    schedulerService.Schedule(
                        new CompiledExpressionSchedulableTask<T>(expression.Compile(), taskTrigger));
                    break;
                default:
                    throw new ArgumentException(
                        $"The expression of '{expression.GetType().FullName}' is not supported.",
                        nameof(expression)
                    );
            }
        }
        
        #endregion
        
    }
    
}