using System;
using System.Text.RegularExpressions;

namespace Scalider.Firebase.Messaging.Target
{
    
    public sealed class Topic : RemoteTarget
    {

        private const string TopicFormat = "/topics/{0}";
        private static readonly Regex TopicNamePattern = new Regex("[a-zA-Z0-9-_.~%]+", RegexOptions.Compiled);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Topic"/> class.
        /// </summary>
        /// <param name="topic"></param>
        public Topic(string topic)
        {
            Check.NotNullOrEmpty(topic, nameof(topic));
            if (!TopicNamePattern.IsMatch(topic))
            {
                throw new ArgumentException(
                    "The format of the topic is invalid",
                    nameof(topic)
                );
            }

            Name = topic;
        }
        
        public string Name { get; }

        public string Value => string.Format(TopicFormat, Name);

    }
    
}