using System.Collections.Generic;
using EA4S.Database;
using EA4S.Profile;

namespace EA4S.Core
{
    public static class AppSceneHelper
    {
        // refactor: scene names should match AppScene so that this can be removed
        public static string GetSceneName(AppScene scene, Database.MiniGameData minigameData = null)
        {
            switch (scene) {
                case AppScene.Home:
                    return "_Start";
                case AppScene.AnturaSpace:
                    return "app_AnturaSpace";
                case AppScene.Book:
                    return "app_Book";
                case AppScene.Map:
                    return "app_Map";
                case AppScene.Mood:
                    return "app_Mood";
                case AppScene.GameSelector:
                    return "app_GamesSelector";
                case AppScene.Intro:
                    return "app_Intro";
                case AppScene.MiniGame:
                    return minigameData.Scene;
                case AppScene.PlayerCreation:
                    return "app_PlayerCreation";
                case AppScene.PlaySessionResult:
                    return "app_PlaySessionResult";
                case AppScene.Rewards:
                    return "app_Rewards";
                case AppScene.ReservedArea:
                    return "app_ReservedArea";
                case AppScene.Ending:
                    return "app_Ending";
                default:
                    return "";
            }
        }
    }

    public struct NavigationData
    {
        public PlayerProfile CurrentPlayer;
        public AppScene CurrentScene;
        public bool RealPlaySession;
        public Stack<AppScene> PrevSceneStack;

        /// <summary>
        /// List of minigames selected for the current play session
        /// </summary>
        public List<MiniGameData> CurrentPlaySessionMiniGames;

        /// <summary>
        /// Current minigame index in
        /// </summary>
        public int CurrentMiniGameIndexInPlaySession { get; private set; }

        public void Setup()
        {
            CurrentScene = AppScene.None;
            PrevSceneStack = new Stack<AppScene>();
        }

        public void Initialize(PlayerProfile _playerProfile)
        {
            CurrentPlayer = _playerProfile;
        }

        public void SetFirstMinigame()
        {
            CurrentMiniGameIndexInPlaySession = 0;
        }

        public bool SetNextMinigame()
        {
            int NextIndex = CurrentMiniGameIndexInPlaySession + 1;
            if (NextIndex < CurrentPlaySessionMiniGames.Count) {
                CurrentMiniGameIndexInPlaySession = NextIndex;
                return true;
            }
            return false;
        }

        public MiniGameData CurrentMiniGameData {
            get {
                if (CurrentPlaySessionMiniGames == null) return null;
                if (CurrentPlaySessionMiniGames.Count == 0) return null;
                return CurrentPlaySessionMiniGames[CurrentMiniGameIndexInPlaySession];
            }
        }
    }

}
