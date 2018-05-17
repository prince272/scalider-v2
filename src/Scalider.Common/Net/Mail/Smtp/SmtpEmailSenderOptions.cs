using JetBrains.Annotations;

namespace Scalider.Net.Mail.Smtp
{

    /// <summary>
    /// Options used by the <see cref="SmtpEmailSender"/> class.
    /// </summary>
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
        public string Host { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the port number for the SMTP server. The default value is 25.
        /// </summary>
        public ushort Port { get; [UsedImplicitly] set; } = 25;

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled for delivering the email messages.
        /// </summary>
        public bool? EnableSsl { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets a value indicating whether the default credentials should be used.
        /// </summary>
        public bool UseDefaultCredentials { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the user name used to login on the SMTP server.
        /// </summary>
        public string UserName { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the password used to login on the SMTP server.
        /// </summary>
        public string Password { get; [UsedImplicitly] set; }

        /// <summary>
        /// Gets or sets the domain or computer name that verifies the credentials.
        /// </summary>
        public string Domain { get; [UsedImplicitly] set; }

    }

}