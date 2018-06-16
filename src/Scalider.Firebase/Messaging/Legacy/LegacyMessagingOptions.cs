using JetBrains.Annotations;

namespace Scalider.Firebase.Messaging.Legacy
{
    
    [UsedImplicitly]
    public class LegacyMessagingOptions
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyMessagingOptions"/> class.
        /// </summary>
        public LegacyMessagingOptions()
        {
        }
        
        [UsedImplicitly]
        public string AuthorizationKey { get; set; }
        
    }
    
}