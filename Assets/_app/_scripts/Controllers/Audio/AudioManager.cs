using UnityEngine;
using System.Collections;

namespace EA4S
{

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;
        static System.Action OnNotifyEndAudio;

        public bool MusicEnabled { get { return musicEnabled; } }

        bool musicEnabled = true;
        Music currentMusic;

        void Awake()
        {
            I = this;
            musicEnabled = true;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // app is pausing
            if (pauseStatus) {
                StopMusic();
            }
            //app is resuming
            if (!pauseStatus) {
                if (musicEnabled) {
                    PlayMusic(currentMusic);
                }
            }
        }

        public void NotifyEndAudio(Fabric.EventNotificationType type, string boh, object info, GameObject gameObject)
        {
            // Debug.Log ("OnNotify:" + type + "GameObject:" + gameObject.name);
            if (info != null) {
                if (type == Fabric.EventNotificationType.OnAudioComponentStopped) {
                    //Debug.Log("NotifyEndAudio OnAudioComponentStopped()");
                    if (OnNotifyEndAudio != null) {
                        OnNotifyEndAudio();
                    }
                }
            }
        }

        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            if (musicEnabled) {
                PlayMusic(currentMusic);
            } else {
                StopMusic();
            }
        }

        public void PlayMusic(Music music)
        {
            currentMusic = music;
            var eventName = AudioConfig.GetMusicEventName(music);
            if (eventName == "") {
                StopMusic();
            } else {
                if (musicEnabled) {
                    Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.SetSwitch, eventName);
                    Fabric.EventManager.Instance.PostEvent("MusicTrigger");
                    //Fabric.EventManager.Instance.PostEvent("Music/" + eventName);
                }
            }
        }

        public void StopMusic()
        {
            Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.StopAll);
        }

        /// <summary>
        /// Play a soundFX
        /// </summary>
        /// <param name="sfx">Sfx.</param>
        public void PlaySfx(Sfx sfx)
        {
            PlaySound(AudioConfig.GetSfxEventName(sfx));
        }

        public void StopSfx(Sfx sfx)
        {
            StopSound(AudioConfig.GetSfxEventName(sfx));
        }

        void PlaySound(string eventName)
        {
            Fabric.EventManager.Instance.PostEvent(eventName);
        }

        void PlaySound(string eventName, System.Action callback)
        {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEventNotify(eventName, NotifyEndAudio);
        }

        void PlaySound(string eventName, GameObject GO)
        {
            Fabric.EventManager.Instance.PostEvent(eventName, GO);
        }

        public void PlayLetter(string letterId)
        {
            Fabric.EventManager.Instance.PostEvent("VOX/Letters/" + letterId);
        }

        public void PlayWord(string wordId)
        {
            Fabric.EventManager.Instance.PostEvent("VOX/Words/" + wordId);
        }

        void StopSound(string eventName)
        {
            if (Fabric.EventManager.Instance != null)
                Fabric.EventManager.Instance.PostEvent(eventName, Fabric.EventAction.StopAll);
        }

        void FadeOutMusic(string n)
        {
            Fabric.Component component = Fabric.FabricManager.Instance.GetComponentByName(n);
            if (component != null) {
                component.FadeOut(0.1f, 0.5f);
            }
        }

        public void PlayDialog(string string_id)
        {
            //Debug.Log("PlayDialog 1: " + string_id + " - " + Fabric.EventManager.GetIDFromEventName(string_id));
            // if (Fabric.EventManager.GetIDFromEventName(string_id) > 0) {
            Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetSwitch, string_id);
            Fabric.EventManager.Instance.PostEvent("KeeperDialog");
            // }
        }

        public void PlayDialog(string string_id, System.Action callback)
        {
            // Debug.Log("PlayDialog 2: " + string_id + " - " + Fabric.EventManager.GetIDFromEventName(string_id));
            //if (Fabric.EventManager.GetIDFromEventName(string_id) > 0) {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetSwitch, string_id);
            Fabric.EventManager.Instance.PostEventNotify("KeeperDialog", NotifyEndAudio);
            //}
        }

    }
}