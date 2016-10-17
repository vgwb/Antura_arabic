using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class TestAudioManager : MonoBehaviour
    {

        public string MyEventName;
        public GameObject[] LEDs;

        void Start()
        {
            MyEventName = AudioConfig.GetSfxEventName(Sfx.AlarmClock);
        }

        public void StartSfxTest(int id)
        {
            Fabric.EventManager.Instance.PostEvent(MyEventName, LEDs[id]);
        }

        public void StopSfxTest(int id)
        {
            //AudioManager.I.StopSfx(Sfx.GameTitle);
            Fabric.EventManager.Instance.PostEvent(MyEventName, Fabric.EventAction.StopSound, LEDs[id]);
            //MyEventName = "";
        }

        public void ChangePitch(float pitch)
        {
            Fabric.EventManager.Instance.PostEvent(MyEventName, Fabric.EventAction.SetPitch, pitch);
        }

        void Update()
        {
            if (Fabric.EventManager.Instance.IsEventActive(MyEventName, LEDs[0])) {
                LEDs[0].SetActive(true);
            } else {
                LEDs[0].SetActive(false);
            }
            if (Fabric.EventManager.Instance.IsEventActive(MyEventName, LEDs[1])) {
                LEDs[1].SetActive(true);
            } else {
                LEDs[1].SetActive(false);
            }
        }
    }
}