using Antura.Audio;
using Antura.Utilities;
using UnityEngine;

namespace Antura
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