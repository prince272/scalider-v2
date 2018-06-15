namespace Scalider.Hosting
{

    internal class NullTaskExceptionHandler : ITaskExceptionHandler
    {

        public static readonly NullTaskExceptionHandler Instance = new NullTaskExceptionHandler();

        #region ITaskExceptionHandler Members

        /// <inheritdoc />
        public void OnUnhandledException(UnhandledTaskExceptionContext context)
        {
            // no-op
        }

        #endregion

    }

}