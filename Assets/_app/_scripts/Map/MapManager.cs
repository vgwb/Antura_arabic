using UnityEngine;

namespace EA4S
{
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