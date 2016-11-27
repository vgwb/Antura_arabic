using UnityEngine;
using System.Collections;
using ModularFramework.Core;
using ModularFramework.Modules;

namespace EA4S
{
    public abstract class MiniGameBase : SubGame
    {
        /// <summary>
        /// If true 
        /// </summary>
        public bool UseTestGameplayInfo;
        public AnturaGameplayInfo GameplayInfo = new AnturaGameplayInfo();

        protected virtual void Start()
        {
            // Navigation manager 
            NavigationManager.I.CurrentScene = AppScene.MiniGame;

            if (!UseTestGameplayInfo)
                GameplayInfo = AppManager.Instance.Modules.GameplayModule.ActualGameplayInfo as AnturaGameplayInfo;
            else // manual set on framework for test session
                AppManager.Instance.Modules.GameplayModule.ActualGameplayInfo = GameplayInfo;
            AppManager.Instance.OnMinigameStart();
            ReadyForGameplay();
        }

        /// <summary>
        /// Invoked at the end of the minigame scene loading and passing the necessary parameters to the gameplay session.
        /// </summary>
        /// <param name="_gameplayInfo"></param>
        protected virtual void ReadyForGameplay()
        {
            //Debug.LogFormat("Gameplay {0} ready with data: {1}", GameplayInfo.GameId, GameplayInfo);
        }

        protected virtual void OnDisable()
        {
            OnMinigameQuit();
        }

        /// <summary>
        /// Invoke when the current minigame ends.
        /// </summary>
        protected virtual void OnMinigameQuit() { }
    }
}