/**
 * here we can put all static general config, to be used with Config.name_of_your_costant;
 *
 **/

namespace EA4S
{
    public static class AppConstants
    {
        public const string AppVersion = "0.11.28b";

#if UNITY_EDITOR

        public const string UrlGithubRepository = "https://github.com/vgwb/EA4S_Antura_U3D";
        public const string UrlTrello = "https://trello.com/b/ltLndaQI/ea4s-beta";

#endif
    }
}
