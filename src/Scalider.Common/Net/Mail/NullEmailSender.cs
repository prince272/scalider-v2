#if NETSTANDARD2_0
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scalider.Net.Mail
{

    /// <summary>
    /// Represents an implementation of the <see cref="IEmailSender"/> interface that doesn't delivers the email
    /// messages but logs them instead.
    /// </summary>
    public sealed class NullEmailSender : IEmailSender
    {

        private readonly ILogger<NullEmailSender> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullEmailSender"/> class.
        /// </summary>
        /// <param name="loggerFactory"></param>
        public NullEmailSender(ILoggerFactory loggerFactory)
        {
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger<NullEmailSender>();
        }

        private void LogMessage(string callingMethod, MailMessage message)
        {
            _logger.LogWarning($"Using {nameof(NullEmailSender)}. The message will not be delivered!");
            _logger.LogDebug(callingMethod);

            _logger.LogDebug($"\tTo: {message.To}");
            _logger.LogDebug($"\tCC: {message.CC}");
            _logger.LogDebug($"\tBCC: {message.Bcc}");
            _logger.LogDebug($"\tSubject: {message.Subject}");
            _logger.LogDebug($"\tBody: {message.Body}");
        }

        #region IEmailSender Members

        /// <inheritdoc />
        public void Send(MailMessage message)
        {
            Check.NotNull(message, nameof(message));

            LogMessage("Send", message);
        }

        /// <inheritdoc />
        public Task SendAsync(MailMessage message, CancellationToken cancellationToken = default)
        {
            Check.NotNull(message, nameof(message));

            LogMessage("SendAsync", message);
            return Task.CompletedTask;
        }

        #endregion

    }

}
#endif