using EA4S.Audio;
using UnityEngine;

namespace EA4S.Map
{
    /// <summary>
    /// Manages the Map scene, from which the next Play Session can be started.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            // Navigation manager 
            AudioManager.I.PlayMusic(SceneMusic);

            //KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro);
        }

        public void GoToAnturaSpace() {
            AppManager.I.NavigationManager.GoToAnturaSpace();
        }
    }
}