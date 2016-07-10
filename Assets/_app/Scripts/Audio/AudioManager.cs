using UnityEngine;
using System.Collections;

namespace EA4S
{
    public static class AudioManager
    {
        static System.Action OnNotifyEndAudio;

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

        public static void PlaySound(string eventName) {
            Fabric.EventManager.Instance.PostEvent(eventName);
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