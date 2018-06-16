using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Scalider.Firebase.Messaging.Target;

namespace Scalider.Firebase.Messaging
{
    
    /// <summary>
    /// Represents a remote Firebase Message.
    /// </summary>
    [UsedImplicitly,
     JsonConverter(typeof(RemoteMessageJsonConverter))]
    public sealed class RemoteMessage
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMessage"/> class.
        /// </summary>
        public RemoteMessage()
        {
        }
        
        [UsedImplicitly]
        public RemoteTarget RemoteTarget { get; set; }
        
        [UsedImplicitly]
        public bool DryRun { get; set; }

        [UsedImplicitly]
        public MessagePriority Priority { get; set; } = MessagePriority.Normal;
        
        [UsedImplicitly]
        public string CollapseKey { get; set; }
        
        [UsedImplicitly]
        public TimeSpan? TimeToLive { get; set; }
        
        [UsedImplicitly]
        public string RestrictedPackageName { get; set; }
        
        [UsedImplicitly]
        public Notification Notification { get; set; }
        
        [UsedImplicitly]
        public object Payload { get; set; }

    }
    
}