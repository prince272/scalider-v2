using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Scalider.Firebase.Messaging
{

    public class ApnNotification
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ApnNotification"/> class.
        /// </summary>
        public ApnNotification()
        {
        }
        
        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.ContentAvailable,
            NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContentAvailable { get; set; }
        
        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.MutableContent,
            NullValueHandling = NullValueHandling.Ignore)]
        public bool? MutableContent { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Sound,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Sound { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Badge,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? BadgeCount { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.ClickAction,
            NullValueHandling = NullValueHandling.Ignore)]
        public string ClickAction { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Subtitle,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Subtitle { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.BodyLocalizationKey,
            NullValueHandling = NullValueHandling.Ignore)]
        public string BodyLozalizationKey { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.BodyLocalizationArgs,
            NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> BodyLocalizationArgs { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.TitleLocalizationKey,
            NullValueHandling = NullValueHandling.Ignore)]
        public string TileLocalizationKey { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.TitleLocalizationArgs,
            NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> TitleLocalizationArgs { get; set; }

    }

}