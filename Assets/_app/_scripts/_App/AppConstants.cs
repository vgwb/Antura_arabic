namespace EA4S.Core
{
    /// <summary>
    /// Container for application-wide static constants.
    /// </summary>
    // refactor: enforce code convention
    // refactor: reorganize all constants (some are statics, others are consts)
    public static class AppConstants
    {
        public const string AppVersion = "0.13.dev";
        public const string DbSchemeVersion = "1.19";
        public const bool DebugPanelEnabled = true;
        public static bool UseUnityAnalytics = false;
        public static bool VerboseLogging = true;
        public static bool DebugLogInserts = false;

        // Urls
        public const string UrlWebsite = "http://www.antura.org";
        public const string UrlPrivacy = "http://www.antura.org/en/privacy-policy/";
        public const string UrlUploadData = "https://upload.antura.org";
        public const string UrlStoreiOSApple = "";
        public const string UrlStoreAndroidGoogle = "";

        // Application details (used by Teacher and maybe other)
        public const float minimumAge = 7;
        public const float maximumAge = 15;

        public const float minimumStage = 1;
        public const float maximumStage = 6;

        public const int minimumMoodValue = 1;
        public const int maximumMoodValue = 5;

        /// <summary>
        /// Max number of player profiles usable.
        /// </summary>
        public const int MaxNumberOfPlayerProfiles = 5;

        // Resource Paths
        public const string AvatarsResourcesDir = "Images/Avatars/";

#if UNITY_EDITOR
        public const string UrlGithubRepository = "https://github.com/vgwb/EA4S_Antura_U3D";
        public const string UrlTrello = "https://trello.com/b/ltLndaQI/ea4s-beta";
#endif
    }
}