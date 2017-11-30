using System.Collections.Generic;
using Antura.Profile;
using UnityEngine.Serialization;

namespace Antura.Core
{
    /// <summary>
    /// Defines settings that must be saved locally.
    /// </summary>
    [System.Serializable]
    public class AppSettings
    {
        // not used anymore.. but could be useful in the future
        public bool HighQualityGfx = false;

        // the uuid of currently active player
        public string LastActivePlayerUUID;

        // to enable english subtitles (especially in the Book)
        public bool EnglishSubtitles = true;

        public bool MusicOn = true;

        // we save the current AppVersion maybe we should compare when installing updated versions
        public string ApplicationVersion;

        // the list of saved players
        public List<PlayerIconData> SavedPlayers = new List<PlayerIconData>();
    }
}