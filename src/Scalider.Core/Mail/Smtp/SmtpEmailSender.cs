using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Scalider.Mail.Smtp
{

    /// <summary>
    /// Represents an implementation of the <see cref="IEmailSender"/> interface that uses SMTP to deliver emails
    /// messages.
    /// </summary>
    [UsedImplicitly,
     SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class SmtpEmailSender : IEmailSender
    {

        private readonly SmtpEmailSenderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
        /// </summary>
        /// <param name="options"></param>
        public SmtpEmailSender(IOptions<SmtpEmailSenderOptions> options)
        {
            _options = options?.Value
                       ?? throw new ArgumentException("The options to connect to the SMTP server are required",
                           nameof(options));
        }

        /// <inheritdoc />
        public virtual void Send(MailMessage message)
        {
            Check.NotNull(message, nameof(message));

            using (var client = CreateClient())
            {
                client.Send(message.Normalize());
            }
        }

        /// <inheritdoc />
        public virtual async Task SendAsync(MailMessage message, CancellationToken cancellationToken = default)
        {
            Check.NotNull(message, nameof(message));

            using (var client = CreateClient())
            {
                await client.SendMailAsync(message.Normalize());
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SmtpClient"/> class.
        /// </summary>
        /// <returns>
        /// The <see cref="SmtpClient"/>.
        /// </returns>
        protected virtual SmtpClient CreateClient()
        {
            var client = new SmtpClient(_options.Host, _options.Port);
            try
            {
                if (_options.EnableSsl.HasValue)
                    client.EnableSsl = _options.EnableSsl.Value;

                // Apply credentials
                if (client.UseDefaultCredentials)
                    client.UseDefaultCredentials = _options.UseDefaultCredentials;
                else
                {
                    client.UseDefaultCredentials = false;

                    if (!string.IsNullOrEmpty(_options.UserName))
                    {
                        client.Credentials = string.IsNullOrEmpty(_options.Domain)
                            ? new NetworkCredential(_options.UserName, _options.Password)
                            : new NetworkCredential(_options.UserName, _options.Password, _options.Domain);
                    }
                }

                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

    }

}