using UnityEngine;
using EA4S.Audio;

namespace EA4S.Scenes
{
    public class PlayerCreationScene : MonoBehaviour
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start()
        {
            if (SceneMusic != Music.Custom) {
                AudioManager.I.PlayMusic(SceneMusic);
            }

        }

    }
}