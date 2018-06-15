using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Scalider.Net.Mail;
using MailMessage = System.Net.Mail.MailMessage;

namespace Scalider.MailKit
{

    /// <summary>
    /// Provides an implementation of the <see cref="IEmailSender"/> interface that uses MailKit to deliver email
    /// messages.
    /// </summary>
    [UsedImplicitly,
     SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class MailKitEmailSender : IEmailSender
    {

        private readonly MailKitEmailSenderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailSender"/> class.
        /// </summary>
        /// <param name="options"></param>
        public MailKitEmailSender(IOptions<MailKitEmailSenderOptions> options)
        {
            _options = options?.Value
                       ?? throw new ArgumentException("The options to connect to the SMTP server are required",
                           nameof(options));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SmtpClient"/> class which is connected, and if needed,
        /// authenticated on the SMTP server.
        /// </summary>
        /// <returns>
        /// The <see cref="SmtpClient"/>.
        /// </returns>
        protected virtual SmtpClient CreateClient() => CreateClientAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously creates a new instance of the <see cref="SmtpClient"/> class which is connected, and if
        /// needed, authenticated on the SMTP server.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        /// <returns>
        /// The <see cref="SmtpClient"/>.
        /// </returns>
        protected virtual async Task<SmtpClient> CreateClientAsync(CancellationToken cancellationToken = default)
        {
            var client = new SmtpClient();

            // Try to connect and authenticate on the remote server
            try
            {
                await client.ConnectAsync(_options.Host, _options.Port, _options.SocketOptions, cancellationToken);
                if (!_options.UseDefaultCredentials && !string.IsNullOrEmpty(_options.UserName))
                {
                    // We should also authenticate using the given credentials
                    await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
                }
            }
            catch
            {
                client.Dispose();
                throw;
            }

            // Done
            return client;
        }

        /// <summary>
        /// Delivers an email message.
        /// </summary>
        /// <param name="message">The message to be delivered.</param>
        public virtual void Send(MimeMessage message)
        {
            Check.NotNull(message, nameof(message));

            using (var client = CreateClient())
            {
                client.Send(message);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Asynchronously delivers an email message.
        /// </summary>
        /// <param name="message">The message to be delviered.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public virtual async Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
        {
            Check.NotNull(message, nameof(message));

            using (var client = await CreateClientAsync(cancellationToken))
            {
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
        }

        void IEmailSender.Send(MailMessage message)
        {
            Check.NotNull(message, nameof(message));

            Send(message.Normalize().ToMimeMessage());
        }

        Task IEmailSender.SendAsync(MailMessage message, CancellationToken cancellationToken)
        {
            Check.NotNull(message, nameof(message));

            return SendAsync(message.Normalize().ToMimeMessage(), cancellationToken);
        }

    }

}