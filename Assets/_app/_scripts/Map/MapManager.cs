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
            NavigationManager.I.CurrentScene = AppScene.Map;
            AudioManager.I.PlayMusic(SceneMusic);

            //KeeperManager.I.PlayDialog(Db.LocalizationDataId.Map_Intro);
        }

    }

}