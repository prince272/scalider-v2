namespace Scalider.Firebase
{
    
    internal static class FirebaseConstants
    {

        public static class Messaging
        {

            public const string FirebaseApiUri = "https://fcm.googleapis.com/";

            public const string PriorityHigh = "high";
            public const string PriorityNormal = "normal";

            public const string To = "to";
            public const string RegistrationIds = "registration_ids";
            public const string Condition = "condition";

            public const string Data = "data";
            public const string Notification = "notification";
            public const string DryRun = "dry_run";
            public const string Priority = "priority";
            public const string CollapseKey = "collapse_key";
            public const string Ttl = "time_to_live";
            public const string RestrictedPackageName = "restricted_package_name";

            public const string ContentAvailable = "content-available";
            public const string MutableContent = "mutable-content";
            public const string ChannelId = "android_channel_id";
            public const string Sound = "sound";
            public const string Badge = "badge";
            public const string Icon = "icon";
            public const string ClickAction = "click_action";
            public const string Color = "color";
            public const string Subtitle = "subtitle";
            public const string Tag = "tag";
            public const string BodyLocalizationKey = "body_loc_key";
            public const string BodyLocalizationArgs = "body_loc_args";
            public const string TitleLocalizationKey = "title_loc_key";
            public const string TitleLocalizationArgs = "title_loc_args";

        }
        
    }
    
}