using UnityEngine;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class MoodManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            NavigationManager.I.CurrentScene = AppScene.Mood;
            AudioManager.I.PlayMusic(SceneMusic);
            KeeperManager.I.PlaySceneIntroduction(NavigationManager.I.CurrentScene);
        }

        /// <summary> 
        /// Mood selected. Values 0,1,2,3,4.
        /// </summary>
        /// <param name="_mood"></param>
        public void MoodSelected(int _mood)
        {
            AppManager.Instance.Teacher.logAI.LogMood(_mood);
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
            Invoke("exitScene", 0.5f);
        }

        void exitScene()
        {
            NavigationManager.I.GoToScene(AppScene.Map);
        }
    }
}