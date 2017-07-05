using UnityEngine;
using EA4S.MinigamesAPI;

#if UNITY_EDITOR

namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// Utility component. When the scene starts from here, 
    /// </summary>
    public class MiniGameAutoLauncher : MonoBehaviour
    {
        public MiniGameCode MiniGameCode;
        public float Difficulty = 0.5f;
        public bool TutorialEnabled = true;
        public int NumberOfRounds = 1;

        public int Stage = 1;
        public int LearningBlock = 1;
        public int PlaySession = 1;
        
        void Start()
        {
            Debug.LogError(AppManager.I.NavigationManager.IsInFirstLoadedScene);
            if (!AppManager.I.NavigationManager.IsInFirstLoadedScene) return;
            AppManager.I.Player.SetCurrentJourneyPosition(Stage, LearningBlock, PlaySession);
            MinigameLaunchConfiguration config = new MinigameLaunchConfiguration(Difficulty, NumberOfRounds, TutorialEnabled);
            AppManager.I.GameLauncher.LaunchGame(MiniGameCode, config, true);
        }
    }

}

#endif