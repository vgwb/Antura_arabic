using UnityEngine;
using ModularFramework.Core;
using System.Collections;

namespace EA4S
{
    public enum AppScene
    {
        Home,
        Mood,
        Map,
        Book,
        Intro,
        GameSelector,
        MiniGame,
        AnturaSpace,
        Rewards
    }

    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager I;

        public AppScene CurrentScene;

        void Start()
        {
            I = this;
        }

        public void GoToScene(AppScene nextScene)
        {
            var nextSceneName = GetSceneName(nextScene);
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition(nextSceneName);
        }

        public string GetNextScene()
        {
            return "";
        }

        public void GoNextScene()
        {
            var nextScene = GetNextScene();
        }

        public string GetCurrentScene()
        {
            return "";
        }

        public void GoHome()
        {

        }

        public void GoBack() { }

        public void ExitCurrentGame() { }

        public string GetSceneName(AppScene scene, Db.MiniGameData minigameData = null)
        {
            switch (scene) {
                case AppScene.Home:
                    return "_Start";
                case AppScene.Mood:
                    return "app_Mood";
                case AppScene.Map:
                    return "app_Map";
                case AppScene.Book:
                    return "app_PlayerBook";
                case AppScene.Intro:
                    return "app_Intro";
                case AppScene.GameSelector:
                    return "app_GamesSelector";
                case AppScene.MiniGame:
                    return minigameData.Scene;
                case AppScene.AnturaSpace:
                    return "app_AnturaSpace";
                case AppScene.Rewards:
                    return "app_Rewards";
                default:
                    return "";
            }
        }
    }
}