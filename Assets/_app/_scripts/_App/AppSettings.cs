using ModularFramework.Modules;

namespace EA4S
{

    public static class AppConstants
    {
        public const string AppVersion = "0.11.28a";
        public const bool DebugPanelEnabled = true;

        // Resource Paths
        public const string AvatarsResourcesDir = "Images/Avatars/";

#if UNITY_EDITOR
        public const string UrlGithubRepository = "https://github.com/vgwb/EA4S_Antura_U3D";
        public const string UrlTrello = "https://trello.com/b/ltLndaQI/ea4s-beta";
#endif
    }

    /// <summary>
    /// Game Setting Extension class.
    /// </summary>
    [System.Serializable]
    public class AppSettings : ModularFramework.Modules.GlobalOptions
    {
        public bool DoLogPlayerBehaviour;
        public bool HighQualityGfx;

        public int LastActivePlayerId;

        public bool UseTestDatabase;
    }
}