using EA4S.Core;

namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// Base class for an in-scene manager of a minigame.
    /// To be derived by specific game scene managers.
    /// </summary>
    // refactor: the dependency on ModularFramework.Core.SubGame is unclear
    // refactor: this should be moved to _minigames/_common 
    public abstract class MiniGameBase : Singleton<MiniGameBase>
    {
        public bool UseTestGameplayInfo;

        // refactor: it is not clear how the GameplayInfo is used, as the core does not mention it
        public AnturaGameplayInfo GameplayInfo = new AnturaGameplayInfo();

        protected virtual void Start()
        {
            if (!UseTestGameplayInfo) {
                GameplayInfo = AppManager.Instance.Modules.GameplayModule.ActualGameplayInfo as AnturaGameplayInfo;
            } else { // manual set on framework for test session
                AppManager.Instance.Modules.GameplayModule.ActualGameplayInfo = GameplayInfo;
            }
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