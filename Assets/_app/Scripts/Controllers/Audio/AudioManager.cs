using UnityEngine;
using System.Collections;
using DG.DeAudio;

namespace EA4S
{
    public enum Music {
        MainTheme,
        Relax
    }

    public enum Sfx {
        Hit
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;

        DeAudioClipData Music1;
        DeAudioClipData Hit;


        static System.Action OnNotifyEndAudio;

        void Awake() {
            I = this;
        }


        public void NotifyEndAudio(Fabric.EventNotificationType type, string boh, object info, GameObject gameObject) {
            // Debug.Log ("OnNotify:" + type + "GameObject:" + gameObject.name);
            if (info != null) {
                if (type == Fabric.EventNotificationType.OnAudioComponentStopped) {
                    if (OnNotifyEndAudio != null) {
                        //Debug.Log ("NotifyEndAudio call custom callback");
                        OnNotifyEndAudio();
                    }
                }
            }
        }

        public void PlayMusic(Music music) {
            var eventName = "";
            switch (music) {
                case Music.MainTheme:
                    eventName = "music1";
                    break;
                case Music.Relax:
                    eventName = "music2";
                    break;
            }

            Fabric.EventManager.Instance.PostEvent("MusicTrigger");
            Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.SetSwitch, eventName);
        }

        public void PlaySfx(Sfx sfx) {
            var eventName = "";
            switch (sfx) {
                case Sfx.Hit:
                    eventName = "Hit";
                    break;
            }

            PlaySound(eventName);
        }

        public void PlaySound(string eventName) {
            Fabric.EventManager.Instance.PostEvent(eventName);
        }

        public void PlaySound(string eventName, System.Action callback) {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEventNotify(eventName, NotifyEndAudio);
        }

        public void PlaySound(string eventName, GameObject GO) {
            Fabric.EventManager.Instance.PostEvent(eventName, GO);
        }

        public void PlayLetter(string wordId) {
            Fabric.EventManager.Instance.PostEvent("VOX/Letters/" + wordId);
        }

        public void PlayWord(string wordId) {
            Fabric.EventManager.Instance.PostEvent("VOX/Words/" + wordId);
        }

        public void StopSound(string n) {
            Fabric.EventManager.Instance.PostEvent(n, Fabric.EventAction.StopAll);
        }

        public void FadeOutMusic(string n) {
            Fabric.Component component = Fabric.FabricManager.Instance.GetComponentByName(n);
            if (component != null) {
                component.FadeOut(0.1f, 0.5f);
            }
        }

    }
}