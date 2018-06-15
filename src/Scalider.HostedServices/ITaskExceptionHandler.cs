using JetBrains.Annotations;

namespace Scalider.Hosting
{
    
    /// <summary>
    /// Provides a way to execute custom logic when an unhandled exception is thrown by a task being executed
    /// on a hosted service.
    /// </summary>
    public interface ITaskExceptionHandler
    {

        /// <summary>
        /// Executed when a task being executed on a hosted service throws an exception.
        /// </summary>
        /// <param name="context">The <see cref="UnhandledTaskExceptionContext"/>.</param>
        void OnUnhandledException([NotNull] UnhandledTaskExceptionContext context);

    }
    
}