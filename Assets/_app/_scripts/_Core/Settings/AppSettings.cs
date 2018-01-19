using Antura.Profile;
using System;
using System.Collections.Generic;

namespace Antura.Core
{
    /// <summary>
    /// Defines app settings that must be saved locally.
    /// </summary>
    [Serializable]
    public class AppSettings
    {
        // not used anymore.. but could be useful in the future
        public bool HighQualityGfx = false;

        // the uuid of currently active player
        public string LastActivePlayerUUID;

        // to enable english subtitles (especially in the Book)
        public bool EnglishSubtitles = true;

        // to enable arabic subtitles / Keeper Widget that shows what he's saying
        public bool SubtitlesEnabled = false;

        public bool MusicEnabled = true;

        public bool OnlineAnalyticsEnabled;

        // we save the current AppVersion maybe we should compare when installing updated versions
        public string AppVersion;

        // the list of saved players
        public List<PlayerIconData> SavedPlayers = new List<PlayerIconData>();

        public void SetAppVersion(Version _version)
        {
            AppVersion = _version.ToString();
        }

        public void DeletePlayers()
        {
            SavedPlayers = new List<PlayerIconData>();
            LastActivePlayerUUID = "";
        }
    }
}