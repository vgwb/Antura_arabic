using EA4S.Audio;
using EA4S.MinigamesCommon;
using EA4S.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Management.Test
{
    /// <summary>
    /// Test class for testing the audio manager.
    /// </summary>
    // refactor: is this needed anymore?
    public class TestAudioManager : MonoBehaviour
    {

        public string MyEventName;
        public GameObject[] LEDs;
        public GameObject PanelMusic;
        public GameObject PanelSfx;
        public GameObject PanelLocalization;
        public GameObject PlayButtonPrefab;

        Sfx currentSfx;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);

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

            btnGO = Instantiate(PlayButtonPrefab);
            btnGO.transform.SetParent(PanelMusic.transform, false);
            btnGO.GetComponentInChildren<Text>().text = "Stop Music";
            btnGO.GetComponent<Button>().onClick.AddListener(StopMusic);

            foreach (Music mus in GenericUtilities.SortEnums<Music>()) {
                //Debug.Log(mus.ToString());
                btnGO = Instantiate(PlayButtonPrefab);
                btnGO.transform.SetParent(PanelMusic.transform, false);
                btnGO.GetComponentInChildren<Text>().text = mus.ToString();
                AddListenerMusic(btnGO.GetComponent<Button>(), mus);
            }

            ///// SFX

            foreach (Transform t in PanelSfx.transform) {
                Destroy(t.gameObject);
            }

            btnGO = Instantiate(PlayButtonPrefab);
            btnGO.transform.SetParent(PanelSfx.transform, false);
            btnGO.GetComponentInChildren<Text>().text = "Stop Sfx";
            btnGO.GetComponent<Button>().onClick.AddListener(StopCurrentSfx);

            foreach (Sfx sfx in GenericUtilities.SortEnums<Sfx>()) {
                //Debug.Log(sfx.ToString());
                btnGO = Instantiate(PlayButtonPrefab);
                btnGO.transform.SetParent(PanelSfx.transform, false);
                btnGO.GetComponentInChildren<Text>().text = sfx.ToString();
                AddListenerSfx(btnGO.GetComponent<Button>(), sfx);
            }

            ///// DIALOG

            foreach (Transform t in PanelLocalization.transform) {
                Destroy(t.gameObject);
            }

            btnGO = Instantiate(PlayButtonPrefab);
            btnGO.transform.SetParent(PanelLocalization.transform, false);
            btnGO.GetComponentInChildren<Text>().text = "Stop Dialog";
            btnGO.GetComponent<Button>().onClick.AddListener(StopCurrentLocalization);

            foreach (var loc in AppManager.I.DB.GetAllLocalizationData()) {
                //Debug.Log(sfx.ToString());
                btnGO = Instantiate(PlayButtonPrefab);
                btnGO.transform.SetParent(PanelLocalization.transform, false);
                btnGO.GetComponentInChildren<Text>().text = loc.GetId();
                AddListenerLocalization(btnGO.GetComponent<Button>(), loc.GetId());
            }
        }


        #region music

        public GameObject PanelMusic1 {
            get { return PanelMusic; }
            set { PanelMusic = value; }
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

        #endregion

        #region Sfx
        IAudioSource lastSfxAudioSource;

        void AddListenerSfx(Button b, Sfx sfx)
        {
            b.onClick.AddListener(() => PlaySfx(sfx));
        }

        void StopCurrentSfx()
        {
            if (lastSfxAudioSource != null)
                lastSfxAudioSource.Stop();
        }
        void PlaySfx(Sfx sfx)
        {
            currentSfx = sfx;
            Debug.Log("playing music :" + currentSfx);
            lastSfxAudioSource = AudioManager.I.PlaySound(currentSfx);
        }

        #endregion

        #region Dialogs
        IAudioSource lastDialogueAudioSource;

        void AddListenerLocalization(Button b, string localizationID)
        {
            b.onClick.AddListener(() => PlayDialog(localizationID));
        }

        void StopCurrentLocalization()
        {
            if (lastDialogueAudioSource != null)
                lastDialogueAudioSource.Stop();
        }
        void PlayDialog(string localizationID)
        {
            Debug.Log("playing localization :" + localizationID);
            lastDialogueAudioSource = AudioManager.I.PlayDialogue(localizationID);
        }

        #endregion

        public void StopAll()
        {
            AudioManager.I.StopMusic();
            AudioManager.I.StopSounds();
            AudioManager.I.StopDialogue(true);
            AudioManager.I.StopLettersWordsPhrases();
        }

        void Update()
        {

            //Fabric.Component[] components = Fabric.FabricManager.Instance.GetComponentsByName(MyEventName, LEDs[0]);

            //if (components != null && components.Length > 0) {
            //    components[0].Volume = 0.5f;

            //    if (components[0].IsPlaying() == true) {
            //        Debug.Log("Component is playing");
            //    }
            //}

            /*
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
            */
        }
    }
}