using UnityEngine;
using System.Collections.Generic;
using CGL.Antura;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using System;
using ModularFramework.Modules;

namespace CGL.Antura.FastCrowd
{

    public class GameDontWakeUp : AnturaMiniGame
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