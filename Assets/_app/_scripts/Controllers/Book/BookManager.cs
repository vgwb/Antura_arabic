using UnityEngine;
using EA4S;
using ModularFramework.Core;

namespace EA4S
{
    public class BookManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            AudioManager.I.PlayMusic(SceneMusic);

            SceneTransitioner.Close();
        }


        public void OpenMap()
        {
            GameManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Map");
        }
    }
}