using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace EA4S
{
    public class TestAudioManager : MonoBehaviour
    {

        public string MyEventName;
        public GameObject[] LEDs;
        public GameObject PanelMusic;
        public GameObject PanelSfx;
        public GameObject PlayButtonPrefab;

        Sfx currentSfx;

        void Start()
        {
            InitUI();
            MyEventName = AudioConfig.GetSfxEventName(Sfx.AlarmClock);
        }

        void InitUI()
        {
            GameObject btnGO;

            //// MUSIC
            foreach (Transform t in PanelMusic.transform) {
                Destroy(t.gameObject);
            }

            btnGO = (GameObject)Instantiate(PlayButtonPrefab);
            btnGO.transform.SetParent(PanelMusic.transform, false);
            btnGO.GetComponentInChildren<Text>().text = "Stop Music";
            btnGO.GetComponent<Button>().onClick.AddListener(() => StopMusic());

            foreach (Music mus in Enum.GetValues(typeof(Music))) {
                //Debug.Log(mus.ToString());
                btnGO = (GameObject)Instantiate(PlayButtonPrefab);
                btnGO.transform.SetParent(PanelMusic.transform, false);
                btnGO.GetComponentInChildren<Text>().text = mus.ToString();
                AddListenerMusic(btnGO.GetComponent<Button>(), mus);
            }

            ///// SFX

            foreach (Transform t in PanelSfx.transform) {
                Destroy(t.gameObject);
            }

            btnGO = (GameObject)Instantiate(PlayButtonPrefab);
            btnGO.transform.SetParent(PanelSfx.transform, false);
            btnGO.GetComponentInChildren<Text>().text = "Stop Sfx";
            btnGO.GetComponent<Button>().onClick.AddListener(() => StopCurrentSfx());

            foreach (Sfx sfx in Enum.GetValues(typeof(Sfx))) {
                //Debug.Log(sfx.ToString());
                btnGO = (GameObject)Instantiate(PlayButtonPrefab);
                btnGO.transform.SetParent(PanelSfx.transform, false);
                btnGO.GetComponentInChildren<Text>().text = sfx.ToString();
                AddListenerSfx(btnGO.GetComponent<Button>(), sfx);
            }
        }

        void AddListenerMusic(Button b, Music music)
        {
            b.onClick.AddListener(() => PlayMusic(music));
        }

        void StopMusic()
        {
            AudioManager.I.StopMusic();
        }

        void PlayMusic(Music music)
        {
            Debug.Log("playing music :" + music);
            AudioManager.I.PlayMusic(music);
        }

        void AddListenerSfx(Button b, Sfx sfx)
        {
            b.onClick.AddListener(() => PlaySfx(sfx));
        }

        void StopCurrentSfx()
        {
            AudioManager.I.StopSfx(currentSfx);
        }
        void PlaySfx(Sfx sfx)
        {
            currentSfx = sfx;
            Debug.Log("playing music :" + currentSfx);
            AudioManager.I.PlaySfx(currentSfx);
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