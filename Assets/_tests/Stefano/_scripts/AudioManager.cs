using UnityEngine;
using System.Collections;
using DG.DeAudio;

namespace EA4S.DeAudio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        public DeAudioClipData Music1;
        public DeAudioClipData Music2;
        public DeAudioClipData Hit;


        static System.Action OnNotifyEndAudio;

        void Awake() {
            Instance = this;
        }


        public static void NotifyEndAudio(Fabric.EventNotificationType type, string boh, object info, GameObject gameObject) {
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

        public void PlayMusic() {
            // Fabric.EventManager.Instance.PostEvent(eventName);
            Music1.Play();
        }

        public void PlayMusic2() {
            DeAudioManager.Crossfade(Music2);
        }



        public void PlayHit() {
            // Fabric.EventManager.Instance.PostEvent(eventName);
            Hit.Play();
        }

        public static void PlaySound(string eventName) {
            // Fabric.EventManager.Instance.PostEvent(eventName);
            // Music1.Play();
        }

        public static void PlaySound(string eventName, System.Action callback) {
            OnNotifyEndAudio = callback;
            Fabric.EventManager.Instance.PostEventNotify(eventName, NotifyEndAudio);
        }

        public static void PlaySound(string eventName, GameObject GO) {
            Fabric.EventManager.Instance.PostEvent(eventName, GO);
        }

        public static void StopSound(string n) {
            Fabric.EventManager.Instance.PostEvent(n, Fabric.EventAction.StopAll);
        }

        public static void FadeOutMusic(string n) {
            Fabric.Component component = Fabric.FabricManager.Instance.GetComponentByName(n);
            if (component != null) {
                component.FadeOut(0.1f, 0.5f);
            }
        }

    }
}