using UnityEngine;
using System.Collections.Generic;
using EA4S;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using System;
using ModularFramework.Modules;

namespace EA4S.DontWakeUp
{

    public class GameDontWakeUp : MiniGameBase
    {

        [Header("Gameplay Info and Config section")]
        #region Overrides

        new public GameDontWakeUpGameplayInfo GameplayInfo;

        new public static GameDontWakeUp Instance
        {
            get { return SubGame.Instance as GameDontWakeUp; }
        }

        #endregion



    }

    /// <summary>
    /// Gameplay info class data structure.
    /// </summary>
    [Serializable]
    public class GameDontWakeUpGameplayInfo : AnturaGameplayInfo
    {
        public float Time = 10;
    }
}