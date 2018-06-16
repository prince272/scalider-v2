using System;
using Newtonsoft.Json;
using Scalider.Firebase.Messaging.Target;

namespace Scalider.Firebase.Messaging
{

    internal class RemoteMessageJsonConverter : JsonConverter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMessageJsonConverter"/> class.
        /// </summary>
        public RemoteMessageJsonConverter()
        {
        }
        
        #region WriteJson

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is RemoteMessage remoteMessage))
                return;

            // Determine if the remote message has a payload or a notification value
            if (remoteMessage.Payload == null && remoteMessage.Notification == null)
                throw new ArgumentException(
                    "The remote message is missing the payload and notification",
                    nameof(value)
                );

            // Start the JSON object
            writer.WriteStartObject();

            // Write the message target
            // TODO
            switch (remoteMessage.RemoteTarget)
            {
                case null:
                    throw new ArgumentException(
                        "The remote message target is missing",
                        nameof(value)
                    );
                default:
                    throw new ArgumentException(
                        "The remote message target is not an allowed target type",
                        nameof(value)
                    );
            }

            // Write all message options
            WritePropertyIfNotNull(writer, FirebaseConstants.Messaging.DryRun, remoteMessage.DryRun);
            WritePropertyIfNotNullOrEmpty(writer, FirebaseConstants.Messaging.CollapseKey, remoteMessage.CollapseKey);
            WritePropertyIfNotNull(writer, FirebaseConstants.Messaging.Data, remoteMessage.Payload);
            WritePropertyIfNotNullOrEmpty(writer, FirebaseConstants.Messaging.RestrictedPackageName,
                remoteMessage.RestrictedPackageName);

            // Write the message priority
            var priorityValue = remoteMessage.Priority == MessagePriority.High
                ? FirebaseConstants.Messaging.PriorityHigh
                : FirebaseConstants.Messaging.PriorityNormal;
            
            writer.WritePropertyName(FirebaseConstants.Messaging.Priority);
            writer.WriteValue(priorityValue);

            // Determine if the message has a time to live
            if (remoteMessage.TimeToLive.HasValue)
            {
                // Determine the amount of seconds to live for the notification
                var secondsToLive = (long)Math.Round(remoteMessage.TimeToLive.Value.TotalSeconds);
                if (secondsToLive > int.MaxValue)
                    secondsToLive = int.MaxValue; // We shouldn't exceed the maximum integer value

                // Determine if the seconds to live is valid
                if (secondsToLive > 0)
                {
                    // We got valid seconds to live!
                    writer.WritePropertyName(FirebaseConstants.Messaging.Ttl);
                    writer.WriteValue(remoteMessage.TimeToLive.Value.TotalSeconds);
                }
            }

            // Determine if the message has a payload
            if (remoteMessage.Payload != null)
            {
                // The message has a payload, serialize and write it as the data
                writer.WritePropertyName(FirebaseConstants.Messaging.Data);
                serializer.Serialize(writer, remoteMessage.Payload);
            }

            // Determine if the message has a notification
            if (remoteMessage.Notification != null)
            {
                // The message has a notification, serialize and write it
                writer.WritePropertyName(FirebaseConstants.Messaging.Notification);
                serializer.Serialize(writer, remoteMessage.Notification);
            }

            // Done
            writer.WriteEndObject();
        }
        
        #endregion

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) => throw new NotImplementedException();

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => false;

        private static void WritePropertyIfNotNull(JsonWriter writer, string propertyName, object value)
        {
            if (value == null)
                return;

            writer.WritePropertyName(propertyName);
            writer.WriteValue(value);
        }

        private static void WritePropertyIfNotNullOrEmpty(JsonWriter writer, string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            writer.WritePropertyName(propertyName);
            writer.WriteValue(value);
        }

    }

}