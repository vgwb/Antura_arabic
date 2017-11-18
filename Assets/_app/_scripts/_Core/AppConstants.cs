using System;
using System.Linq;

namespace Antura.Core
{
    /// <summary>
    /// Container for application-wide static constants.
    /// </summary>
    // TODO refactor: enforce code convention
    // TODO refactor: reorganize all constants (some are statics, others are consts)
    public static class AppConstants
    {
        /// <summary>
        /// Version of the application. Displayed in the Home scene.
        /// </summary>
        public const string AppVersion = "1.1.0a5";

        #region Debug Options

        /// <summary>
        /// Enabled the Advanced Debug Panel.
        /// Set to FALSE for production.
        /// </summary>
        public static bool DebugPanelEnabledAtStartup = true;

        /// <summary>
        /// Tracks common events using Unity Analytics.
        /// Set to TRUE for production.
        /// </summary>
        public static bool UnityAnalyticsEnabled = false;

        /// <summary>
        /// Switches on all Debug.Log calls for performance.
        /// Set to FALSE for production.
        /// </summary>
        public static bool DebugLogEnabled = true;

        /// <summary>
        /// Logs all MySQL database inserts.
        /// Set to FALSE for production.
        /// </summary>
        public static bool DebugLogDbInserts = false;

        #endregion

        #region Application Constants

        /// <summary>
        /// Version of the Static Database Scheme.
        /// v1.0.7 - added ArabicFemale to LocalizationData
        /// </summary>
        public const string StaticDbSchemeVersion = "1.0.7";

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

        public const string UrlGithubRepository = "https://github.com/vgwb/Antura_arabic";
        public const string UrlDeveloperDocs = "https://vgwb.github.io/Antura_arabic/";

        // files
        public const string PdfAndroidInstall = "AndroidInstallHelp.pdf";

        // the directories of exported / imported databases
        public const string DbPlayersFolder = "players";
        public const string DbExportFolder = "export";
        public const string DbImportFolder = "import";
        public const string DbJoinedFolder = "joined";

        // Range and Constrain values
        public const float MinPlayerAge = 4;
        public const float MaxPlayerAge = 10;
        public const int MinStage = 1;
        public const int MaxStage = 6;
        public const int MinMoodValue = 1;
        public const int MaxMoodValue = 5;
        public const int MaxNumberOfPlayerProfiles = 5;
        public const int MinMiniGameScore = 0;
        public const int MaxMiniGameScore = 3;

        // used for float to float comparisons
        public const float EPSILON = 0.00001f;

        // Language specifics
        public static readonly TextDirection TextDirection = TextDirection.RightToLeft;

        // Resource Paths
        public const string AvatarsResourcesDir = "Images/Avatars/";

        #endregion

        public static string GetPlayerUUIDFromDatabaseFilename(string fileName)
        {
            return fileName.Split('/').Last().Split('\\').Last().Replace("Antura_Player_", "").Replace(".sqlite3", "");
        }

        public static string GetPlayerDatabaseFilename(string playerUuid)
        {
            return "Antura_Player_" + playerUuid + ".sqlite3";
        }

        public static string GetPlayerDatabaseFilenameForExport(string playerUuid)
        {
            return "export_Antura_Player_" + playerUuid + "_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm") + ".sqlite3";
        }

        public static string GetJoinedDatabaseFilename()
        {
            return "Antura_Joined_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm") + ".sqlite3";
        }
    }
}