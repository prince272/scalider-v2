using System;
using JetBrains.Annotations;

namespace Scalider.Hosting
{
    
    /// <summary>
    /// Provides information on the unhandled task exception.
    /// </summary>
    public class UnhandledTaskExceptionContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledTaskExceptionContext"/> class.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="serviceProvider"></param>
        public UnhandledTaskExceptionContext([NotNull] Exception exception,
            [NotNull] IServiceProvider serviceProvider)
        {
            Check.NotNull(exception, nameof(exception));
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            Exception = exception as AggregateException ?? new AggregateException(exception);
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the Exception that went unhandled.
        /// </summary>
        [UsedImplicitly]
        public AggregateException Exception { get; }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/>
        /// </summary>
        [UsedImplicitly]
        public IServiceProvider ServiceProvider { get; }
        
        /// <summary>
        /// Gets a value indicating  whether this exception has been marked as "handled".
        /// </summary>
        [UsedImplicitly]
        public bool IsHandled { get; private set; }

        /// <summary>
        /// Marks the <see cref="Exception"/> has "handled", thus preventing it from triggering exception escalation
        /// policy which, by default, terminates the process.
        /// </summary>
        [UsedImplicitly]
        public void SetHandled()
        {
            IsHandled = true;
        }

    }
    
}