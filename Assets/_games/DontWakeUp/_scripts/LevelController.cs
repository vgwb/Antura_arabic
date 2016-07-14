using UnityEngine;
using System.Collections;

namespace EA4S.DontWakeUp
{
    public class LevelController : MonoBehaviour
    {
        public GameObject LevelCamera;
        public GameObject Marker1;
        public GameObject Marker2;

        void Start() {
	
        }

        public void SetWord() {
            string wordCode = GameDontWakeUp.Instance.currentWord._id;
            BroadcastMessage("Init", wordCode, SendMessageOptions.DontRequireReceiver);
        }
    }
}