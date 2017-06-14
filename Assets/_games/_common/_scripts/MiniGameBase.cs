using EA4S.Core;

namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// Base class for an in-scene manager of a minigame.
    /// To be derived by specific game scene managers.
    /// </summary>
    // refactor: this should be moved to _minigames/_common 
    public abstract class MiniGameBase : Singleton<MiniGameBase>
    {
        protected virtual void Start()
        {
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