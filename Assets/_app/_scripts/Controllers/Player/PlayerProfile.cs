using System;
using UnityEngine.UI;
using ModularFramework.Modules;
using UnityEngine;

namespace EA4S {

    [Serializable]
    public class PlayerProfile : IPlayerProfile {

        public string Key { get; set; }
        public int Id;
        public int AvatarId;
        public int Age;
        public string Name;

        // Mood (1 to 5 indicators)
        public float MainMood = 3f;
        public float Impatient = 3f;
        public float Impulsive = 3f;
        public float Genius = 3f;
        public float Bored = 3f;
        public float Collector = 3f;
        public float Frustrated = 3f;

        // PlaySkills
        public float Precision;
        public float Reaction;
        public float Memory;
        public float Logic;
        public float Rhythm;
        public float Musicality;
        public float Sight;

        public JourneyPosition MaxJourneyPosition;
        public JourneyPosition CurrentJourneyPosition;
        public int CurrentMiniGameInPlaySession;

        #region Oldies
        public int AnturaCurrentPreset;

        #region Mood
        /// <summary>
        /// False if not executed start mood eval.
        /// </summary>
        [HideInInspector]
        public bool StartMood = false;
        /// <summary>
        /// Start Mood value. Values 0,1,2,3,4.
        /// </summary>
        [HideInInspector]
        public int StartMoodEval = 0;
        /// <summary>
        /// End Mood value. Values 0,1,2,3,4.
        /// </summary>
        [HideInInspector]
        public int EndMoodEval = 0;
        #endregion

        public void Reset() {
            AnturaCurrentPreset = 0;
            CurrentJourneyPosition = new JourneyPosition(1,1,1);
            CurrentMiniGameInPlaySession = 0;
        }
        #endregion
    }
}