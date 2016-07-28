using UnityEngine;
using System.Collections;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class HomeManager : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start() {
            AudioManager.I.PlayMusic(SceneMusic);

            ContinueScreen.Show(Play, ContinueScreenMode.Button);
        }

        public void Play() {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Mood");
        }



    }
}