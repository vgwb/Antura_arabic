using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class TestAudioManager : MonoBehaviour
    {

        public string MyEventName;
        public GameObject LED;

        public void StartSfxTest()
        {
            MyEventName = AudioConfig.GetSfxEventName(Sfx.GameTitle);
            Fabric.EventManager.Instance.PostEvent(MyEventName, gameObject);
        }

        public void StopSfxTest()
        {
            AudioManager.I.StopSfx(Sfx.GameTitle);
            MyEventName = "";
        }

        public void ChangePitch(float pitch)
        {
            Fabric.EventManager.Instance.PostEvent(MyEventName, Fabric.EventAction.SetPitch, pitch);
        }

        void Update()
        {
            if (MyEventName != "") {
                if (Fabric.EventManager.Instance.IsEventActive(MyEventName, gameObject)) {
                    LED.SetActive(true);
                } else {
                    LED.SetActive(false);
                }
            }

        }
    }
}