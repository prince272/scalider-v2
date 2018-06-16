using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Scalider.Firebase.Messaging
{
    
    public class Notification
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        public Notification()
        {
        }
        
        [UsedImplicitly,
         JsonProperty("title")]
        public string Title { get; set; }
        
        [UsedImplicitly,
         JsonProperty("body")]
        public string Body { get; set; }
        
    }
    
}