using UnityEngine;
using System.Collections.Generic;
using EA4S.Database;
using DG.DeAudio;
using EA4S.Core;
using EA4S.Helpers;
using EA4S.MinigamesCommon;

namespace EA4S.Audio
{
    /// <summary>
    /// Handles audio requests throughout the application
    /// </summary>
    public class AudioManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static AudioManager I;

        public bool IsAppPaused { get; private set; }

        List<AudioSourceWrapper> playingAudio = new List<AudioSourceWrapper>();

        DeAudioGroup musicGroup;
        DeAudioGroup wordsLettersPhrasesGroup;
        DeAudioGroup keeperGroup;
        DeAudioGroup sfxGroup;

        Dictionary<IAudioSource, System.Action> dialogueEndedCallbacks = new Dictionary<IAudioSource, System.Action>();

        bool previousMusicEnabled = true;
        bool musicEnabled = true;
        AudioClip customMusic;
        Music currentMusic;
        public bool MusicEnabled {
            get {
                return musicEnabled;
            }

            set {
                if (musicEnabled == value)
                    return;

                musicEnabled = value;

                if (musicEnabled && (currentMusic != Music.Silence)) {
                    if (musicGroup != null) {
                        musicGroup.Resume();

                        bool hasToReset = false;

                        if (musicGroup.sources == null)
                            hasToReset = true;
                        else {
                            foreach (var s in musicGroup.sources) {
                                if (s.isPlaying)
                                    goto Cont;
                            }
                            hasToReset = true;
                        }
                    Cont:
                        if (hasToReset) {
                            if (currentMusic == Music.Custom)
                                musicGroup.Play(customMusic, 1, 1, true);
                            else
                                musicGroup.Play(GetAudioClip(currentMusic), 1, 1, true);
                        }

                    }
                } else {
                    if (musicGroup != null)
                        musicGroup.Pause();
                }
            }
        }

        Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();

        #region Serialized Configuration
        [SerializeField, HideInInspector]
        List<SfxConfiguration> sfxConfs = new List<SfxConfiguration>();

        [SerializeField, HideInInspector]
        List<MusicConfiguration> musicConfs = new List<MusicConfiguration>();

        Dictionary<Sfx, SfxConfiguration> sfxConfigurationMap = new Dictionary<Sfx, SfxConfiguration>();
        Dictionary<Music, MusicConfiguration> musicConfigurationMap = new Dictionary<Music, MusicConfiguration>();

        public void ClearConfiguration()
        {
            sfxConfs.Clear();
            musicConfs.Clear();
            sfxConfigurationMap.Clear();
            musicConfigurationMap.Clear();
        }

        public void UpdateSfxConfiguration(SfxConfiguration conf)
        {
            var id = sfxConfs.FindIndex((a) => { return a.sfx == conf.sfx; });

            if (id >= 0)
                sfxConfs.RemoveAt(id);

            sfxConfs.Add(conf);
            sfxConfigurationMap[conf.sfx] = conf;
        }

        public void UpdateMusicConfiguration(MusicConfiguration conf)
        {
            var id = musicConfs.FindIndex((a) => { return a.music == conf.music; });

            if (id >= 0)
                musicConfs.RemoveAt(id);

            musicConfs.Add(conf);
            musicConfigurationMap[conf.music] = conf;
        }

        public MusicConfiguration GetMusicConfiguration(Music music)
        {
            MusicConfiguration v;
            if (musicConfigurationMap.TryGetValue(music, out v))
                return v;
            return null;
        }

        public SfxConfiguration GetSfxConfiguration(Sfx sfx)
        {
            SfxConfiguration v;
            if (sfxConfigurationMap.TryGetValue(sfx, out v))
                return v;
            return null;
        }
        #endregion

        void Awake()
        {
            I = this;

            sfxGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.FX);
            musicGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Music);
            wordsLettersPhrasesGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Custom0);
            keeperGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Custom1);

            musicEnabled = true;
        }

        public void OnAppPause(bool pauseStatus)
        {
            if (pauseStatus) {
                previousMusicEnabled = MusicEnabled;
                MusicEnabled = false;
            } else {
                MusicEnabled = previousMusicEnabled;
            }

            IsAppPaused = pauseStatus;
        }

        #region Music
        public void ToggleMusic()
        {
            MusicEnabled = !musicEnabled;
        }

        public void PlayMusic(Music newMusic)
        {
            if (currentMusic != newMusic) {
                currentMusic = newMusic;
                musicGroup.Stop();

                var musicClip = GetAudioClip(currentMusic);

                if (currentMusic == Music.Silence || musicClip == null) {
                    StopMusic();
                } else {
                    if (musicEnabled) {
                        musicGroup.Play(musicClip, 1, 1, true);
                    } else {
                        musicGroup.Stop();
                    }
                }
            }
        }

        public void StopMusic()
        {
            currentMusic = Music.Silence;
            musicGroup.Stop();
        }
        #endregion

        #region Sfx
        /// <summary>
        /// Play a soundFX
        /// </summary>
        /// <param name="sfx">Sfx.</param>
        public IAudioSource PlaySound(Sfx sfx)
        {
            AudioClip clip = GetAudioClip(sfx);
            var source = new AudioSourceWrapper(sfxGroup.Play(clip), sfxGroup, this);

            var conf = GetConfiguration(sfx);

            if (conf != null) {
                source.Pitch = 1 + ((Random.value - 0.5f) * conf.randomPitchOffset) * 2;
                source.Volume = conf.volume;
            }

            return source;
        }

        public void StopSounds()
        {
            sfxGroup.Stop();
        }
        #endregion

        #region Letters, Words and Phrases
        public IAudioSource PlayLetter(LetterData data, bool exclusive = true)
        {
            if (exclusive)
                AudioManager.I.StopLettersWordsPhrases();

            AudioClip clip = GetAudioClip(data);
            return new AudioSourceWrapper(wordsLettersPhrasesGroup.Play(clip), wordsLettersPhrasesGroup, this);
        }

        public IAudioSource PlayWord(WordData data, bool exclusive = true)
        {
            if (exclusive)
                AudioManager.I.StopLettersWordsPhrases();

            AudioClip clip = GetAudioClip(data);
            return new AudioSourceWrapper(wordsLettersPhrasesGroup.Play(clip), wordsLettersPhrasesGroup, this);
        }

        public IAudioSource PlayPhrase(PhraseData data, bool exclusive = true)
        {
            if (exclusive)
                AudioManager.I.StopLettersWordsPhrases();

            AudioClip clip = GetAudioClip(data);
            return new AudioSourceWrapper(wordsLettersPhrasesGroup.Play(clip), wordsLettersPhrasesGroup, this);
        }

        public void StopLettersWordsPhrases()
        {
            if (wordsLettersPhrasesGroup != null)
                wordsLettersPhrasesGroup.Stop();
        }
        #endregion

        #region Dialogue
        public IAudioSource PlayDialogue(string localizationData_id)
        {
            return PlayDialogue(LocalizationManager.GetLocalizationData(localizationData_id));
        }

        public IAudioSource PlayDialogue(Database.LocalizationDataId id)
        {
            return PlayDialogue(LocalizationManager.GetLocalizationData(id));
        }

        public IAudioSource PlayDialogue(Database.LocalizationData data, bool clearPreviousCallback = false)
        {
            Debug.Log("PlayDialogue " + data.Id);

            if (clearPreviousCallback)
                dialogueEndedCallbacks.Clear();

            if (!string.IsNullOrEmpty(data.GetLocalizedAudioFileName((AppManager.Instance as AppManager).Player.Gender))) {
                AudioClip clip = GetAudioClip(data);
                return new AudioSourceWrapper(keeperGroup.Play(clip), keeperGroup, this);
            }
            return null;
        }

        public IAudioSource PlayDialogue(string localizationData_id, System.Action callback)
        {
            return PlayDialogue(LocalizationManager.GetLocalizationData(localizationData_id), callback);
        }

        public IAudioSource PlayDialogue(Database.LocalizationDataId id, System.Action callback, bool clearPreviousCallback = false)
        {
            return PlayDialogue(LocalizationManager.GetLocalizationData(id), callback, clearPreviousCallback);
        }

        public IAudioSource PlayDialogue(Database.LocalizationData data, System.Action callback, bool clearPreviousCallback = false)
        {
            if (clearPreviousCallback)
                dialogueEndedCallbacks.Clear();

            if (!string.IsNullOrEmpty(data.GetLocalizedAudioFileName((AppManager.Instance as AppManager).Player.Gender))) {
                AudioClip clip = GetAudioClip(data);
                var wrapper = new AudioSourceWrapper(keeperGroup.Play(clip), keeperGroup, this);
                if (callback != null)
                    dialogueEndedCallbacks[wrapper] = callback;
                return wrapper;
            } else {
                if (callback != null)
                    callback();
            }
            return null;
        }

        public void StopDialogue(bool clearPreviousCallback)
        {
            if (clearPreviousCallback)
                dialogueEndedCallbacks.Clear();

            keeperGroup.Stop();
        }
        #endregion

        #region Audio clip management

        public AudioClip GetAudioClip(LocalizationData data)
        {
            var localizedAudioFileName = data.GetLocalizedAudioFileName((AppManager.Instance as AppManager).Player.Gender);
            var res = GetCachedResource("AudioArabic/Dialogs/" + localizedAudioFileName);
            
            // Fallback to neutral version if not found
            if (res == null)
            {
                var neutralAudioFileName = data.GetLocalizedAudioFileName(PlayerGender.M);
                if (localizedAudioFileName != neutralAudioFileName)
                {
                    Debug.LogWarning("Female audio file expected for localization ID " + data.Id + " was not found");
                    res = GetCachedResource("AudioArabic/Dialogs/" + neutralAudioFileName);
                }
            }

            return res;
        }

        public AudioClip GetAudioClip(LetterData data)
        {
            var res = GetCachedResource("AudioArabic/Letters/" + data.Id);

            if (res == null)
                Debug.Log("Warning: cannot find audio clip for " + data);

            return res;
        }

        public AudioClip GetAudioClip(WordData data)
        {
            var res = GetCachedResource("AudioArabic/Words/" + data.Id);

            if (res == null)
                Debug.Log("Warning: cannot find audio clip for " + data);

            return res;
        }

        public AudioClip GetAudioClip(PhraseData data)
        {
            var res = GetCachedResource("AudioArabic/Phrases/" + data.Id);

            if (res == null)
                Debug.Log("Warning: cannot find audio clip for " + data);

            return res;
        }

        public AudioClip GetAudioClip(Sfx sfx)
        {
            SfxConfiguration conf = GetSfxConfiguration(sfx);

            if (conf == null || conf.clips == null || conf.clips.Count == 0) {
                Debug.Log("No Audio clips configured for: " + sfx);
                return null;
            }

            return conf.clips.GetRandom();
        }

        public SfxConfiguration GetConfiguration(Sfx sfx)
        {
            SfxConfiguration conf = GetSfxConfiguration(sfx);

            if (conf == null || conf.clips == null || conf.clips.Count == 0) {
                Debug.Log("No Audio clips configured for: " + sfx);
                return null;
            }

            return conf;
        }

        public AudioClip GetAudioClip(Music music)
        {
            MusicConfiguration conf = GetMusicConfiguration(music);

            if (conf == null)
                return null;

            return conf.clip;
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
        #endregion

        List<KeyValuePair<AudioSourceWrapper, System.Action>> pendingCallbacks = new List<KeyValuePair<AudioSourceWrapper, System.Action>>();
        public void Update()
        {
            for (int i = 0; i < playingAudio.Count; ++i) {
                var source = playingAudio[i];
                if (source.Update()) {
                    // could be collected
                    playingAudio.RemoveAt(i--);

                    System.Action callback;
                    if (source.Group == keeperGroup && dialogueEndedCallbacks.TryGetValue(source, out callback)) {
                        pendingCallbacks.Add(new KeyValuePair<AudioSourceWrapper, System.Action>(source, callback));
                    }
                }
            }

            for (int i = 0; i < pendingCallbacks.Count; ++i) {
                pendingCallbacks[i].Value();
                dialogueEndedCallbacks.Remove(pendingCallbacks[i].Key);
            }

            pendingCallbacks.Clear();
        }

        public void OnAfterDeserialize()
        {
            // Update map from serialized data
            sfxConfigurationMap.Clear();
            for (int i = 0, count = sfxConfs.Count; i < count; ++i)
                sfxConfigurationMap[sfxConfs[i].sfx] = sfxConfs[i];

            musicConfigurationMap.Clear();
            for (int i = 0, count = musicConfs.Count; i < count; ++i)
                musicConfigurationMap[musicConfs[i].music] = musicConfs[i];
        }

        public void OnBeforeSerialize()
        {

        }

        public IAudioSource PlaySound(AudioClip clip)
        {
            return new AudioSourceWrapper(sfxGroup.Play(clip), sfxGroup, this);
        }

        public IAudioSource PlayMusic(AudioClip clip)
        {
            StopMusic();
            currentMusic = Music.Custom;

            var source = musicGroup.Play(clip);

            customMusic = clip;

            return new AudioSourceWrapper(source, musicGroup, this);
        }

        /// <summary>
        /// Used by AudioSourceWrappers.
        /// </summary>
        /// <param name="source"></param>
        public void OnAudioStarted(AudioSourceWrapper source)
        {
            if (!playingAudio.Contains(source))
                playingAudio.Add(source);
        }
    }
}