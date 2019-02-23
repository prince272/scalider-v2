using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Hosting.Queue.Internal;

namespace Scalider.Hosting.Queue
{

    /// <summary>
    /// Provides extension methods for the <see cref="ITaskQueueService"/> interface.
    /// </summary>
    public static class TaskQueueServiceExtensions
    {

        /// <summary>
        /// Adds a function delegate to the end of the queue.
        /// </summary>
        /// <param name="queueService">The <see cref="ITaskQueueService"/>.</param>
        /// <param name="func">The function to add to the queue.</param>
        [UsedImplicitly]
        public static void Enqueue([NotNull] this ITaskQueueService queueService, [NotNull] Func<Task> func)
        {
            Check.NotNull(queueService, nameof(queueService));
            Check.NotNull(func, nameof(func));

            queueService.Enqueue(new DelegateQueueableTask(ctx => func()));
        }

        /// <summary>
        /// Adds a function delegate to the end of the queue.
        /// </summary>
        /// <param name="queueService">The <see cref="ITaskQueueService"/>.</param>
        /// <param name="func">The function to add to the queue.</param>
        [UsedImplicitly]
        public static void Enqueue([NotNull] this ITaskQueueService queueService,
            [NotNull] Func<QueuedTaskExecutionContext, Task> func)
        {
            Check.NotNull(queueService, nameof(queueService));
            Check.NotNull(func, nameof(func));

            queueService.Enqueue(new DelegateQueueableTask(func));
        }


        /// <summary>
        /// Adds a queued task that queries or create a service of <typeparamref name="T"/> and executes the
        /// method or retrieves and executes the delegate using the member accessor.
        ///
        /// If <typeparamref name="T" /> is not registered as a service, then a new instance is created when the
        /// task is executed.
        /// </summary>
        /// <typeparam name="T">The type of the delegate that the <see cref="Expression{TDelegate}"/>
        /// represents.</typeparam>
        /// <param name="queueService">The <see cref="ITaskQueueService"/>.</param>
        /// <param name="expression">The expression.</param>
        /// <exception cref="ArgumentException">When the <paramref name="expression"/> is invalid.</exception>
        [UsedImplicitly]
        public static void Enqueue<T>([NotNull] this ITaskQueueService queueService,
            [NotNull] Expression<Func<T, Func<QueuedTaskExecutionContext, Task>>> expression)
            where T : class
        {
            Check.NotNull(queueService, nameof(queueService));
            Check.NotNull(expression, nameof(expression));

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
                        queueService.Enqueue(new StaticMemberAccessQueueableTask(memberExpression.Member));
                        return;
                    }

                    queueService.Enqueue(new CompiledExpressionQueueableTask<T>(expression.Compile()));
                    break;
                case MethodCallExpression methodCallExpression:
                    if (methodCallExpression.Object is ConstantExpression methodConstantExpression &&
                        methodConstantExpression.Value is MethodInfo methodInfo &&
                        methodInfo.IsStatic)
                    {
                        // The method call is to a static method, no need to compile the expression
                        queueService.Enqueue(new StaticMethodInvokationQueueableTask(methodInfo));
                        return;
                    }

                    queueService.Enqueue(new CompiledExpressionQueueableTask<T>(expression.Compile()));
                    break;
                default:
                    throw new ArgumentException(
                        $"The expression of '{expression.GetType().FullName}' is not supported.",
                        nameof(expression)
                    );
            }
        }

    }

}