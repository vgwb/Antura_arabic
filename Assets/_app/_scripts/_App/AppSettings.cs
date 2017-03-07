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
        public bool HighQualityGfx;

        public string LastActivePlayerUUID;

        public bool UseTestDatabase;

        public bool EnglishSubtitles = false;

        public List<PlayerIconData> SavedPlayers = new List<PlayerIconData>();
    }
}