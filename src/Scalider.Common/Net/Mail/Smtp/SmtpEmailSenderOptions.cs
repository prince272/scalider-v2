using JetBrains.Annotations;

namespace Scalider.Net.Mail.Smtp
{

    /// <summary>
    /// Options used by the <see cref="SmtpEmailSender"/> class.
    /// </summary>
    [UsedImplicitly]
    public class SmtpEmailSenderOptions
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
        /// </summary>
        public SmtpEmailSenderOptions()
        {
        }

        /// <summary>
        /// Gets or sets the host for the SMTP server.
        /// </summary>
        [UsedImplicitly]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port number for the SMTP server. The default value is 25.
        /// </summary>
        [UsedImplicitly]
        public ushort Port { get; set; } = 25;

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled for delivering the email messages.
        /// </summary>
        [UsedImplicitly]
        public bool? EnableSsl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the default credentials should be used.
        /// </summary>
        [UsedImplicitly]
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets the user name used to login on the SMTP server.
        /// </summary>
        [UsedImplicitly]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used to login on the SMTP server.
        /// </summary>
        [UsedImplicitly]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the domain or computer name that verifies the credentials.
        /// </summary>
        [UsedImplicitly]
        public string Domain { get; set; }

    }

}