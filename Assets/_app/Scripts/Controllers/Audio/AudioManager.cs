using UnityEngine;
using System.Collections;
using DG.DeAudio;

namespace EA4S
{
    public enum Music {
        MainTheme,
        Relax,
        Theme3,
        Theme4
    }

    public enum Sfx {
        Hit,
        UIPopup,
        UIButtonClick,
        UIPauseIn,
        UIPauseOut,
        CameraMovement,
        AlarmClock,
        LetterAngry,
        LetterHappy,
        LetterSad
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
                    eventName = "Music1";
                    break;
                case Music.Relax:
                    eventName = "Music2";
                    break;
                case Music.Theme3:
                    eventName = "Music3";
                    break;
                case Music.Theme4:
                    eventName = "Music4";
                    break;
            }

            Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.SetSwitch, eventName);
            Fabric.EventManager.Instance.PostEvent("MusicTrigger");
            //Fabric.EventManager.Instance.PostEvent("Music/" + eventName);
        }

        public void PlaySfx(Sfx sfx) {
            var eventName = "";
            switch (sfx) {
                case Sfx.Hit:
                    eventName = "Hit";
                    break;
                case Sfx.UIPopup:
                    eventName = "Sfx/UI/Popup";
                    break;
                case Sfx.UIButtonClick:
                    eventName = "Sfx/UI/Button";
                    break;
                case Sfx.UIPauseIn:
                    eventName = "Sfx/UI/PauseIn";
                    break;
                case Sfx.UIPauseOut:
                    eventName = "Sfx/UI/PauseOut";
                    break;
                case Sfx.CameraMovement:
                    eventName = "Sfx/CameraMovement";
                    break;
                case Sfx.AlarmClock:
                    eventName = "Sfx/AlarmClock";
                    break;
                case Sfx.LetterAngry:
                    eventName = "LivingLetter/Angry";
                    break;  
                case Sfx.LetterHappy:
                    eventName = "LivingLetter/Happy";
                    break;  
                case Sfx.LetterSad:
                    eventName = "LivingLetter/Sad";
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