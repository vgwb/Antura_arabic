using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S {

    public abstract class AnturaMiniGame : SubGame {
        /// <summary>
        /// If true 
        /// </summary>
        public bool UseTestGameplayInfo;
        public AnturaGameplayInfo GameplayInfo = new AnturaGameplayInfo();
        void Start() {
            if (!UseTestGameplayInfo)
                GameplayInfo = AnturaGameManager.Instance.Modules.GameplayModule.ActualGameplayInfo as AnturaGameplayInfo;
            else // manual set on framework for test session
                AnturaGameManager.Instance.Modules.GameplayModule.ActualGameplayInfo = GameplayInfo;
            ReadyForGameplay();
        }

        /// <summary>
        /// Invoked at the end of the minigame scene loading and passing the necessary parameters to the gameplay session.
        /// </summary>
        /// <param name="_gameplayInfo"></param>
        protected virtual void ReadyForGameplay() {
            //Debug.LogFormat("Gameplay {0} ready with data: {1}", GameplayInfo.GameId, GameplayInfo);
        }
    }
}