namespace EA4S.Core
{
    /// <summary>
    /// Container for application-wide static constants.
    /// </summary>
    // refactor: enforce code convention
    // refactor: reorganize all constants (some are statics, others are consts)
    public static class AppConstants
    {

        /// <summary>
        /// Version of the application. Displayed in the Home scene.
        /// </summary>
        public const string AppVersion = "1.0.1 (1)";

        #region Debug Options

        /// <summary>
        /// Enabled the Advanced Debug Panel.
        /// Set to FALSE for production.
        /// </summary>
        public const bool DebugPanelEnabled = true;

        /// <summary>
        /// Tracks common events using Unity Analytics.
        /// Set to TRUE for production.
        /// </summary>
        public static bool UseUnityAnalytics = false;

        /// <summary>
        /// Switches on all Debug.Log calls for performance.
        /// Set to FALSE for production.
        /// </summary>
        public static bool VerboseLogging = true;

        /// <summary>
        /// Logs all MySQL database inserts.
        /// Set to FALSE for production.
        /// </summary>
        public static bool DebugLogInserts = false;

        /// <summary>
        /// Stops a MiniGame from playing if the PlaySession database does not allow that MiniGame to be played at a given position.
        /// Used only for debug purposes (with the Debug Panel)
        /// </summary>
        public static bool DebugStopPlayAtWrongPlaySessions = true;

        #endregion

        #region Application Constants

        /// <summary>
        /// Version of the Static Database Scheme.
        /// </summary>
        public const string StaticDbSchemeVersion = "1.0.0";

        /// <summary>
        /// Version of the MySQL Database Scheme.
        /// @note: Change with EXTREME CAUTION, as the MySQL databases are regenerated (and thus the data is removed) when a change is detected.
        /// </summary>
        public const string DynamicDbSchemeVersion = "1.0.0";

        // public URLs
        public const string UrlWebsite = "http://www.antura.org";
        public const string UrlPrivacy = "http://www.antura.org/en/privacy-policy/";
        public const string UrlUploadData = "https://upload.antura.org";
        public const string UrlStoreiOSApple = "https://itunes.apple.com/us/app/antura-and-the-letters/id1210334699?ls=1&mt=8";
        public const string UrlStoreAndroidGoogle = "https://play.google.com/store/apps/details?id=org.eduapp4syria.antura";
        public const string UrlCommunityTelegram = "https://t.me/antura";
        public const string UrlCommunityFacebook = "https://www.facebook.com/antura.initiative";

        public const string UrlSupportForm = "https://docs.google.com/forms/d/e/1FAIpQLScWxs5I0w-k8GlIgPFKoWBitMVJ9gxxJlKvGKOXzZsnAA0qNw/viewform";

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
        public const int minimumMinigameScore = 0;
        public const int maximumMinigameScore = 3;

        // Resource Paths
        public const string AvatarsResourcesDir = "Images/Avatars/";

        #endregion
    }
}