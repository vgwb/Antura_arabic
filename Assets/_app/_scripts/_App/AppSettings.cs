using System.Collections.Generic;
using EA4S.Profile;

namespace EA4S.Core
{
    /// <summary>
    /// Game Setting Extension class.
    /// </summary>
    [System.Serializable]
    public class AppSettings : ModularFramework.Modules.GlobalOptions
    {
        // not used anymore.. but could be useful in the future
        public bool HighQualityGfx;

        // the uuid of currently active player
        public string LastActivePlayerUUID;

        // to enable english subtitles (especially in the Book)
        public bool EnglishSubtitles;

        // we save the current AppVersion maybe we shhould compare when installing updated versions
        public string ApplicationVersion;

        // the list of saved players
        public List<PlayerIconData> SavedPlayers = new List<PlayerIconData>();

        // used for development
        public bool UseTestDatabase;
    }
}