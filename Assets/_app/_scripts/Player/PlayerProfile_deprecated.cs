using UnityEngine;
using System.Collections;

namespace EA4S
{
    /// <summary>
    /// TODO: to be deleted during final app lifecycle refactoring.
    /// </summary>
    public class PlayerProfile_deprecated
    {
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

        public PlayerProfile_deprecated()
        {
            Reset();
        }

        public void Reset()
        {
            AnturaCurrentPreset = 0;
        }
    }
}