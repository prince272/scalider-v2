using JetBrains.Annotations;
using MailKit.Security;

namespace Scalider.MailKit
{
    
    /// <summary>
    /// Options used by the <see cref="MailKitEmailSender"/> class.
    /// </summary>
    public class MailKitEmailSenderOptions
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailSenderOptions"/> class.
        /// </summary>
        public MailKitEmailSenderOptions()
        {
        }

        /// <summary>
        /// Gets or sets the host for the SMTP server.
        /// </summary>
        public string Host { get; [UsedImplicitly] set; }
        
        /// <summary>
        /// Gets or sets the port number for the SMTP server. The default value is 25.
        /// </summary>
        public short Port { get; [UsedImplicitly] set; } = 25;

        /// <summary>
        /// Gets or sets the secure socket options to when connecting to the SMTP server. The default value is
        /// <see cref="SecureSocketOptions.Auto"/>.
        /// </summary>
        public SecureSocketOptions SocketOptions { get; [UsedImplicitly] set; } = SecureSocketOptions.Auto;
        
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

    }
    
}