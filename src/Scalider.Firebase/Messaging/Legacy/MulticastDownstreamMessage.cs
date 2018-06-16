using System;
using Newtonsoft.Json.Linq;

namespace Scalider.Firebase.Messaging.Legacy
{
    
    public class MulticastDownstreamMessage : IDownstreamMessage
    {
        
        private MulticastDownstreamMessage()
        {
        }

        /// <inheritdoc />
        public string Id { get; private set; }
        
        /// <summary>
        /// Gets a value indicating the unique identifier for the multicast downstream message.
        /// </summary>
        public long MulticastId { get; private set; }
        
        public int Success { get; private set; }
        
        public int Failure { get; private set; }
        
        public int CanonicalIds { get; private set; }

        public static MulticastDownstreamMessage FromJsonObject(JObject jsonObject)
        {
            Check.NotNull(jsonObject, nameof(jsonObject));
            
            // Retrieve and validate all the required parameters
            if (!jsonObject.TryGetValue("multicast_id", StringComparison.OrdinalIgnoreCase, out var multicastIdValue) ||
                !long.TryParse(multicastIdValue.ToObject<string>(), out var multicastId))
            {
            }

            if (!jsonObject.TryGetValue("success", StringComparison.OrdinalIgnoreCase, out var successValue) ||
                !int.TryParse(successValue.ToObject<string>(), out var successCount))
            {
            }

            if (!jsonObject.TryGetValue("failure", StringComparison.OrdinalIgnoreCase, out var failureValue) ||
                !int.TryParse(failureValue.ToObject<string>(), out var failureCount))
            {
            }

            if (!jsonObject.TryGetValue("canonical_ids", StringComparison.OrdinalIgnoreCase,
                    out var canonicalIdsValue) ||
                !int.TryParse(canonicalIdsValue.ToObject<string>(), out var canonicalIdsCount))
            {
            }

            // Done
            return new MulticastDownstreamMessage
            {
                Id = multicastIdValue.ToObject<string>(),
                MulticastId = multicastId,
                Success = successCount,
                Failure = failureCount,
                CanonicalIds = canonicalIdsCount
            };
        }

    }
    
}