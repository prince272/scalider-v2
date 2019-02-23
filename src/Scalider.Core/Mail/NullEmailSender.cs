using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Scalider.Mail
{

    /// <summary>
    /// Represents an implementation of the <see cref="IEmailSender"/> interface that doesn't delivers the email
    /// messages but logs them instead.
    /// </summary>
    [UsedImplicitly]
    public sealed class NullEmailSender : IEmailSender
    {

        private readonly ILogger<NullEmailSender> _logger;

        /// <summary>
        /// Provides a singleton instance of the <see cref="NullEmailSender"/> class.
        /// </summary>
        public static readonly IEmailSender Instance =
            new NullEmailSender(NullLoggerFactory.Instance.CreateLogger<NullEmailSender>());

        /// <summary>
        /// Initializes a new instance of the <see cref="NullEmailSender"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public NullEmailSender(ILogger<NullEmailSender> logger)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
        }

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

    }

}