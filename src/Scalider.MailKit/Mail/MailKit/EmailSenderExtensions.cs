using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MimeKit;

namespace Scalider.Mail.MailKit
{

    /// <summary>
    /// Provides extension methods for the <see cref="IEmailSender"/> interface.
    /// </summary>
    public static class EmailSenderExtensions
    {

        /// <summary>
        /// Delivers an email message.
        /// </summary>
        /// <param name="sender">The <see cref="IEmailSender"/>.</param>
        /// <param name="message">The message to be delivered.</param>
        public static void Send([NotNull] this IEmailSender sender, [NotNull] MimeMessage message)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(message, nameof(message));

            GetMailKitEmailSenderOrThrow(sender).Send(message);
        }

        /// <summary>
        /// Asynchronously delivers an email message.
        /// </summary>
        /// <param name="sender">The <see cref="IEmailSender"/>.</param>
        /// <param name="message">The message to be delviered.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task SendAsync([NotNull] this IEmailSender sender, [NotNull] MimeMessage message,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(sender, nameof(sender));
            Check.NotNull(message, nameof(message));

            return GetMailKitEmailSenderOrThrow(sender).SendAsync(message, cancellationToken);
        }

        private static MailKitEmailSender GetMailKitEmailSenderOrThrow(IEmailSender sender)
        {
            if (sender is MailKitEmailSender kitEmailSender)
                return kitEmailSender;

            // The type of the sender doesn't support MailKit
            var typeName = nameof(MailKitEmailSender);
            throw new ArgumentException(
                $"The email sender must be an instance of the {typeName} class",
                nameof(sender)
            );
        }

    }

}