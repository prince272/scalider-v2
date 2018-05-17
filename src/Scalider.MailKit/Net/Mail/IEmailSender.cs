#if NETSTANDARD1_7
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MimeKit;

namespace Scalider.Net.Mail
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
        [UsedImplicitly]
        void Send([NotNull] MimeMessage message);

        /// <summary>
        /// Asynchronously delivers an email message.
        /// </summary>
        /// <param name="message">The message to be delviered.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [UsedImplicitly]
        Task SendAsync([NotNull] MimeMessage message, CancellationToken cancellationToken = default);

    }

}
#endif