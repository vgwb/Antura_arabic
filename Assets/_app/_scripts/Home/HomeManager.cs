using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager I;

        [Header("Scene Setup")]
        public Music SceneMusic;
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;
        public LLAnimationStates LLAnimation = LLAnimationStates.LL_dancing;
        [Header("References")]
        public AnturaAnimationController AnturaAnimController;
        public LetterObjectView LLAnimController;

        void Awake()
        {
            I = this;
        }

        void Start()
        {
            NavigationManager.I.CurrentScene = AppScene.Home;

            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlayMusic(SceneMusic);
            AudioManager.I.PlaySfx(Sfx.GameTitle);

            AnturaAnimController.State = AnturaAnimation;
            LLAnimController.State = LLAnimation;
        }

        public void Play()
        {
            Debug.Log("Play with Player ID: " + AppManager.I.Player.Id);

            GlobalUI.ShowPauseMenu(true);

            LogManager.I.InitNewSession();
            LogManager.I.LogInfo(InfoEvent.AppPlay, JsonUtility.ToJson(new AppInfoParameters()));

            if (AppManager.I.Player.IsFirstContact()) {
                NavigationManager.I.GoToScene(AppScene.Intro);
            } else {
                if (AppManager.I.Player.MoodLastVisit == System.DateTime.Today.ToString())
                    NavigationManager.I.GoToScene(AppScene.Map);
                else
                    NavigationManager.I.GoToScene(AppScene.Mood);
            }

        }
    }
}