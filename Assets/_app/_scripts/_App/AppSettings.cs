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
        public bool DoLogPlayerBehaviour;
        public bool HighQualityGfx;

        public int LastActivePlayerId;

        public bool UseTestDatabase;

        public bool CheatSuperDogMode = false;

        public bool EnglishSubtitles = false;

        public List<PlayerIcon> PlayerIcons;
    }
}