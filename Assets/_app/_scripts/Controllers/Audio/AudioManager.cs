using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine.Audio;

namespace EA4S
{

    public class AudioManager : MonoBehaviour
    {
        const string LETTERS_PREFIX = "VOX/Letter/";
        const string WORDS_PREFIX = "VOX_Words_";

        public AudioMixerGroup musicGroup;
        public AudioMixerGroup sfxGroup;
        public AudioMixerGroup lettersGroup;

        public static AudioManager I;
        static System.Action OnNotifyEndAudio;

        public bool MusicEnabled { get { return musicEnabled; } }

        bool musicEnabled = true;
        Music currentMusic;

        Dictionary<string, Fabric.AudioComponent> eventToComponent = new Dictionary<string, Fabric.AudioComponent>();
        Dictionary<string, Fabric.RandomComponent> eventToRndComponent = new Dictionary<string, Fabric.RandomComponent>();

        Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();

        void Awake()
        {
            I = this;

            musicEnabled = true;

            // Collect all Event name -> Audio clip pairs
            var components = transform.GetComponentsInChildren<AudioComponent>(true);
            foreach (var c in components) {
                var listener = c.GetComponent<EventListener>();

                if (listener != null)
                    eventToComponent[listener._eventName] = c;
            }

            var rndcomponents = transform.GetComponentsInChildren<RandomComponent>(true);
            foreach (var c in rndcomponents) {
                var listener = c.GetComponent<EventListener>();

                if (listener != null)
                    eventToRndComponent[listener._eventName] = c;
            }
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
            Fabric.EventManager.Instance.PostEvent(LETTERS_PREFIX + letterId);
        }

        public void PlayWord(string wordId)
        {
            //Debug.Log("PlayWord: " + wordId);
            Fabric.EventManager.Instance.PostEvent("Words", Fabric.EventAction.SetAudioClipReference, "Words/" + wordId);
            Fabric.EventManager.Instance.PostEvent("Words");
            //Fabric.EventManager.Instance.PostEvent(WORDS_PREFIX + wordId);
        }

        public void PlayPhrase(string phraseId)
        {
            //Debug.Log("PlayWord: " + wordId);
            Fabric.EventManager.Instance.PostEvent("Words", Fabric.EventAction.SetAudioClipReference, "Phrases/" + phraseId);
            Fabric.EventManager.Instance.PostEvent("Words");
            //Fabric.EventManager.Instance.PostEvent(WORDS_PREFIX + wordId);
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
            var data = AppManager.Instance.DB.GetLocalizationDataById(string_id);
            if (data.AudioFile != "") {
                Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetAudioClipReference, "Dialogs/" + string_id);
                Fabric.EventManager.Instance.PostEvent("KeeperDialog");
            }
        }

        public void PlayDialog(string string_id, System.Action callback)
        {
            var data = AppManager.Instance.DB.GetLocalizationDataById(string_id);
            if (data.AudioFile != "") {
                // Debug.Log("PlayDialog with Callback: " + string_id + " - " + Fabric.EventManager.GetIDFromEventName(string_id));
                OnNotifyEndAudio = callback;
                Fabric.EventManager.Instance.PostEvent("KeeperDialog", Fabric.EventAction.SetAudioClipReference, "Dialogs/" + data.AudioFile);
                Fabric.EventManager.Instance.PostEventNotify("KeeperDialog", NotifyEndAudio);
            } else {
                Debug.Log("PlayDialog() - no audio file.. calling callback() direct");
                callback();
            }
        }
        public AudioClip GetAudioClip(ILivingLetterData data)
        {
            if (data.DataType == LivingLetterDataType.Letter)
                return GetAudioClip(LETTERS_PREFIX + data.Id);
            else if (data.DataType == LivingLetterDataType.Word) {
                return GetCachedResource("AudioArabic/Words/" + WORDS_PREFIX + data.Id);
            }
            return null;
        }

        public AudioClip GetAudioClip(Sfx sfx)
        {
            return GetAudioClip(AudioConfig.GetSfxEventName(sfx));
        }

        AudioClip GetAudioClip(string eventName)
        {
            Fabric.AudioComponent audioComponent = null;

            if (eventToComponent.TryGetValue(eventName, out audioComponent)) {
                return audioComponent.AudioClip;
            }

            Fabric.RandomComponent rndComponent = null;

            if (eventToRndComponent.TryGetValue(eventName, out rndComponent)) {
                var child = rndComponent.GetChildComponents();

                Fabric.AudioComponent c = child.GetRandom() as AudioComponent;
                if (c != null)
                    return c.AudioClip;
            }

            return null;
        }

        AudioClip GetCachedResource(string resource)
        {
            AudioClip clip = null;

            if (audioCache.TryGetValue(resource, out clip))
                return clip;

            clip = Resources.Load(resource) as AudioClip;

            audioCache[resource] = clip;
            return clip;
        }

        public void ClearCache()
        {
            foreach (var r in audioCache)
                Resources.UnloadAsset(r.Value);
            audioCache.Clear();
        }
    }
}