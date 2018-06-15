using System.Net.Mail;
using System.Text;
using JetBrains.Annotations;

namespace Scalider.Net.Mail
{

    /// <summary>
    /// Provides extension methods for the <see cref="IEmailSender"/> interface and the <see cref="MailMessage"/>
    /// class.
    /// </summary>
    public static class EmailExtensions
    {

        /// <summary>
        /// Applies some normalization to the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The <see cref="MailMessage"/> to normalize.</param>
        /// <returns>
        /// The normalized <see cref="MailMessage"/>.
        /// </returns>
        public static MailMessage Normalize([NotNull] this MailMessage message)
        {
            Check.NotNull(message, nameof(message));

            // Ensure that the message encoding are always set, if not, set them to UTF-8
            if (message.HeadersEncoding == null)
                message.HeadersEncoding = Encoding.UTF8;

            if (message.SubjectEncoding == null)
                message.SubjectEncoding = Encoding.UTF8;

            if (message.BodyEncoding == null)
                message.BodyEncoding = Encoding.UTF8;

            // Done
            return message;
        }

    }

}