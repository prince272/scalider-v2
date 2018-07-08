using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Mail
{

    /// <summary>
    /// Defines the basic functionality of a service used to deliver email messages.
    /// </summary>
    public interface IEmailSender
    {

        /// <summary>
        /// Delivers an email message.
        /// </summary>
        /// <param name="message">The message to be delivered.</param>
        void Send([NotNull] MailMessage message);

        /// <summary>
        /// Asynchronously delivers an email message.
        /// </summary>
        /// <param name="message">The message to be delviered.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task SendAsync([NotNull] MailMessage message, CancellationToken cancellationToken = default);

    }

}