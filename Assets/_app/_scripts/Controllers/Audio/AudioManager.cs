using UnityEngine;
using System.Collections;

namespace EA4S
{
    public enum Music {
        Silence = 0,
        MainTheme = 1,
        Relax = 2,
        Lullaby = 3,
        Theme3 = 4,
        Theme4 = 5
    }

    public enum Sfx {
        Hit,
        BaloonPop,
        DangerClock,
        GameTitle,
        Win,
        Lose,
        UIPopup,
        UIButtonClick,
        UIPauseIn,
        UIPauseOut,
        CameraMovement,
        AlarmClock,
        LetterAngry,
        LetterHappy,
        LetterSad,
        WalkieTalkie,
        WheelStart,
        WheelTick
    }


    public class AudioManager : MonoBehaviour
    {
        public static AudioManager I;
        static System.Action OnNotifyEndAudio;

        void Awake() {
            I = this;
        }

        public void NotifyEndAudio(Fabric.EventNotificationType type, string boh, object info, GameObject gameObject) {
            // Debug.Log ("OnNotify:" + type + "GameObject:" + gameObject.name);
            if (info != null) {
                if (type == Fabric.EventNotificationType.OnAudioComponentStopped) {
                    Debug.Log("NotifyEndAudio OnAudioComponentStopped()");
                    if (OnNotifyEndAudio != null) {
                        OnNotifyEndAudio();
                    }
                }
            }
        }

        public void PlayMusic(Music music) {
            var eventName = "";
            switch (music) {
                case Music.Silence:
                    eventName = "";
                    break;
                case Music.MainTheme:
                    eventName = "Music1";
                    break;
                case Music.Relax:
                    eventName = "Music2";
                    break;
                case Music.Lullaby:
                    eventName = "Music5";
                    break;
                case Music.Theme3:
                    eventName = "Music3";
                    break;
                case Music.Theme4:
                    eventName = "Music4";
                    break;
            }

            if (eventName == "") {
                StopMusic();
            } else {
                Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.SetSwitch, eventName);
                Fabric.EventManager.Instance.PostEvent("MusicTrigger");
                //Fabric.EventManager.Instance.PostEvent("Music/" + eventName);
            }
        }

        public void StopMusic() {
            Fabric.EventManager.Instance.PostEvent("MusicTrigger", Fabric.EventAction.StopAll);
        }

        public void PlaySfx(Sfx sfx) {
            PlaySound(GetEventName(sfx));
        }

        public void StopSfx(Sfx sfx) {
            StopSound(GetEventName(sfx));
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

        public void StopSound(string eventName) {
            Fabric.EventManager.Instance.PostEvent(eventName, Fabric.EventAction.StopAll);
        }

        public void FadeOutMusic(string n) {
            Fabric.Component component = Fabric.FabricManager.Instance.GetComponentByName(n);
            if (component != null) {
                component.FadeOut(0.1f, 0.5f);
            }
        }

        public void PlayDialog(string string_id) {
            Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetSwitch, string_id);
        }

        public void PlayDialog(string string_id, System.Action callback) {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetSwitch, string_id);
            Fabric.EventManager.Instance.PostEventNotify("KeeperDialog", NotifyEndAudio);
        }


        string GetEventName(Sfx sfx) {
            var eventName = "";
            switch (sfx) {
                case Sfx.Hit:
                    eventName = "Sfx/Hit";
                    break;
                case Sfx.DangerClock:
                    eventName = "Sfx/DangerClock";
                    break;
                case Sfx.Win:
                    eventName = "Sfx/Win";
                    break;
                case Sfx.Lose:
                    eventName = "Sfx/Lose";
                    break;
                case Sfx.BaloonPop:
                    eventName = "Sfx/BalloonPop";
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
                case Sfx.GameTitle:
                    eventName = "VOX/GameTitle";
                    break;
                case Sfx.WalkieTalkie:
                    eventName = "Sfx/WalkieTalkie";
                    break;
                case Sfx.WheelStart:
                    eventName = "Sfx/WheelStart";
                    break;  
                case Sfx.WheelTick:
                    eventName = "Sfx/WheelTick";
                    break;  
            }
            return eventName;
        }

    }
}