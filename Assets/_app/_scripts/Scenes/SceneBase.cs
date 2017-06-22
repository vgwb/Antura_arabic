using EA4S.Audio;
using EA4S.Utilities;
using UnityEngine;

namespace EA4S
{
    public class SceneBase : SingletonMonoBehaviour<SceneBase>
    {
        [Header("Scene Setup")]
        public Music SceneMusic;

        protected virtual void Start()
        {
            AudioManager.I.PlayMusic(SceneMusic);
        }
    }
}