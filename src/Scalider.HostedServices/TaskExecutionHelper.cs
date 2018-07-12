using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scalider.Hosting
{
    
    internal static class TaskExecutionHelper
    {
        
        #region ReportUnhandledTaskException

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public static void ReportUnhandledTaskException(Exception exception, object task,
            ITaskExceptionHandler taskExceptionHandler, IServiceProvider serviceProvider, ILogger logger)
        {
            if (exception == null)
            {
                // There was no exception to report
                return;
            }
            
            var context = new UnhandledTaskExceptionContext(exception, serviceProvider);
            if (task is ITaskExceptionHandler queueableTaskExceptionHandler)
            {
                // The task implements an exception handler, we will let it handle the exception by itself
                queueableTaskExceptionHandler.OnUnhandledException(context);
            }

            // Determine if the task handled the exception
            if (context.IsHandled)
                return;

            // The task did not handle the exception, we will notify the global exception handler
            taskExceptionHandler?.OnUnhandledException(context);
            if (context.IsHandled)
            {
                // The exception was handled by the global handler
                return;
            }
                
            // Log the unhandled exception
            logger.LogError(exception, "An unhandled exception has occurred while executing the task.");
        }
        
        #endregion

        #region ExecuteExpressionCompilationOnInstance<TArg>
        
        public static Task ExecuteExpressionCompilationOnInstance<T, TArg>(IServiceProvider serviceProvider,
            Func<T, Func<TArg, Task>> taskFactoryCalback, TArg contextArg)
        {
            var instance = serviceProvider.GetService<T>();
            var isInstanceCreatedFromService = instance != null;
            if (instance == null)
            {
                // Create a new instance using the service provider
                instance = ActivatorUtilities.CreateInstance<T>(serviceProvider);
            }

            // Execute the task
            Task task;
            try
            {
                task = taskFactoryCalback(instance)(contextArg) ?? Task.CompletedTask;
            }
            catch
            {
                if (!isInstanceCreatedFromService && instance is IDisposable disposable1)
                {
                    // We created a new instance and the instance is disposable, get rid of it after the task
                    // execution completes
                    disposable1.Dispose();
                }

                throw;
            }

            // Determine if we need to register a continuation for the task to dispose the instnace
            if (!isInstanceCreatedFromService && instance is IDisposable disposable2)
            {
                // We created a new instance and the instance is disposable, get rid of it after the task
                // execution completes
                task = task.ContinueWith((t, s) =>
                {
                    ((IDisposable)s).Dispose();

                    // Determine if the task exited with an exception
                    if (t.Exception != null)
                    {
                        // The task exited with an exception, rethrow the exception
                        throw t.Exception;
                    }
                }, disposable2);
            }

            // Done
            return task;
        }
        
        #endregion

        public static Func<TArg, Task> GetStaticMemberCallback<TArg>(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(null) as Func<TArg, Task>;
                case PropertyInfo propertyInfo when propertyInfo.GetMethod != null:
                    return propertyInfo.GetMethod.Invoke(null, new object[0]) as Func<TArg, Task>;
            }

            return null;
        }

    }
    
}