using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S
{
    public class HomeManager : MonoBehaviour
    {

        [Header("Scene Setup")]
        public Music SceneMusic;

        void Start() {
            AudioManager.I.PlayMusic(SceneMusic);
        }
	
    }
}