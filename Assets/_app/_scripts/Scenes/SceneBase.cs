using EA4S.Audio;
using EA4S.Utilities;
using UnityEngine;

namespace EA4S
{
    public class SceneBase : SingletonMonoBehaviour<SceneBase>
    {
        [Header("Base Scene Setup")]
        public Music SceneMusic;

        protected virtual void Start()
        {
            if (SceneMusic != Music.DontChange) {
                AudioManager.I.PlayMusic(SceneMusic);
            }
        }
    }
}