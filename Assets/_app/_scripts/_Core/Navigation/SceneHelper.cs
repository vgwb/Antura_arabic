using Antura.Database;

namespace Antura.Core
{
    public static class SceneHelper
    {
        // TODO refactor: scene names should match AppScene so that this can be removed
        public static string GetSceneName(AppScene scene, MiniGameData minigameData = null)
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
                case AppScene.DailyReward:
                    return "app_DailyReward";
                default:
                    return "";
            }
        }
    }

}