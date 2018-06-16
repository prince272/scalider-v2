using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Scalider.Firebase.Messaging
{
    
    [UsedImplicitly]
    public class AndroidNotification : Notification
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidNotification"/> class.
        /// </summary>
        public AndroidNotification()
        {
        }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Icon,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? Icon { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Sound,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Sound { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.ClickAction,
            NullValueHandling = NullValueHandling.Ignore)]
        public string ClickAction { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Color,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [UsedImplicitly,
         JsonProperty(FirebaseConstants.Messaging.Tag,
            NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

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