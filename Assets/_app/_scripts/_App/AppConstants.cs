namespace EA4S.Core
{
    /// <summary>
    /// Container for application-wide static constants.
    /// </summary>
    // refactor: enforce code convention
    // refactor: reorganize all constants (some are statics, others are consts)
    public static class AppConstants
    {
        #region Application Constants

        /// <summary>
        /// Version of the application. Displayed in the Home scene.
        /// </summary>
        public const string AppVersion = "0.227";

        // this is used to track changes in the sqlite db scheme.. CHANGE WITH CAUTION!
        // because the db gets wiped when changed
        public const string DbSchemeVersion = "1.21";

        // public Urls
        public const string UrlWebsite = "http://www.antura.org";
        public const string UrlPrivacy = "http://www.antura.org/en/privacy-policy/";
        public const string UrlUploadData = "https://upload.antura.org";
        public const string UrlStoreiOSApple = "";
        public const string UrlStoreAndroidGoogle = "";
        public const string UrlCommunityTelegram = "https://t.me/antura";
        public const string UrlCommunityFacebook = "https://www.facebook.com/antura.initiative";
#if UNITY_EDITOR
        public const string UrlGithubRepository = "https://github.com/vgwb/EA4S_Antura_U3D";
        public const string UrlTrello = "https://trello.com/b/ltLndaQI/ea4s-beta";
#endif

        // Application details (used by Teacher and maybe other)
        public const float minimumAge = 4;
        public const float maximumAge = 10;
        public const float minimumStage = 1;
        public const float maximumStage = 6;
        public const int minimumMoodValue = 1;
        public const int maximumMoodValue = 5;
        public const int MaxNumberOfPlayerProfiles = 5;

        // Resource Paths
        public const string AvatarsResourcesDir = "Images/Avatars/";

        #endregion


        #region Debug Options

        // PRODUCTION: FALSE (enables the advanced Debug Panel)
        public const bool DebugPanelEnabled = true;
        // PRODUCTION: TRUE (tracks common events with Unity Analytics)
        public static bool UseUnityAnalytics = false;
        // PRODUCTION: FALSE (switches off all Debug.Log)
        public static bool VerboseLogging = true;

        public static bool DebugLogInserts = false;
        public static bool DebugStopPlayAtWrongPlaySessions = true;

        #endregion

    }
}