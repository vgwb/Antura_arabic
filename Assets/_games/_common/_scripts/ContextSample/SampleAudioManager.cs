using System;
using DG.DeAudio;
using UnityEngine;
using System.Collections.Generic;

namespace EA4S
{
    public class SampleAudioManager : IAudioManager
    {
        List<SampleAudioSource> playingAudio = new List<SampleAudioSource>();

        DeAudioGroup musicGroup;
        DeAudioGroup wordsLettersGroup;
        DeAudioGroup sfxGroup;

        Music currentMusic;
        public bool MusicEnabled
        {
            get
            {
                return AudioManager.I.MusicEnabled;
            }

            set
            {
                if (AudioManager.I.MusicEnabled != value)
                    AudioManager.I.ToggleMusic();

                if (AudioManager.I.MusicEnabled)
                {
                    PlayMusic(currentMusic);

                    if (musicGroup != null)
                        musicGroup.Resume();
                }
                else
                {
                    StopMusic();

                    if (musicGroup != null)
                        musicGroup.Pause();
                }
            }
        }

        public IAudioSource PlayLetterData(ILivingLetterData id, bool stopAllLetters = false)
        {
            AudioClip clip = AudioManager.I.GetAudioClip(id);

            if (wordsLettersGroup == null)
            {
                wordsLettersGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Custom0);
                wordsLettersGroup.mixerGroup = AudioManager.I.lettersGroup;
            }

            if (stopAllLetters)
                wordsLettersGroup.Stop();

            var source = wordsLettersGroup.Play(clip);

            return new SampleAudioSource(source, wordsLettersGroup, this);

        }

        public void PlayText(Db.LocalizationDataId text)
        {
            PlayDialogue(text, null);
        }

        public void PlayDialogue(Db.LocalizationDataId text, System.Action callback = null)
        {
            if (callback == null)
                AudioManager.I.PlayDialog(text.ToString());
            else
                AudioManager.I.PlayDialog(text.ToString(), callback);
        }

        public void PlayMusic(Music music)
        {
            StopMusic();

            currentMusic = music;

            if (MusicEnabled)
                AudioManager.I.PlayMusic(music);
        }

        public IAudioSource PlaySound(Sfx sfx)
        {
            AudioClip clip = AudioManager.I.GetAudioClip(sfx);

            if (sfxGroup == null)
            {
                sfxGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.FX);
                sfxGroup.mixerGroup = AudioManager.I.sfxGroup;
            }

            var source = sfxGroup.Play(clip);

            return new SampleAudioSource(source, sfxGroup, this);
        }

        public UnityEngine.AudioClip GetAudioClip(Sfx sfx)
        {
            return AudioManager.I.GetAudioClip(sfx);
        }

        public void StopMusic()
        {
            if (musicGroup != null)
                musicGroup.Stop();

            AudioManager.I.StopMusic();
        }

        public void Reset()
        {
            StopMusic();

            if (wordsLettersGroup != null)
                wordsLettersGroup.Stop();

            AudioManager.I.ClearCache();
            AudioManager.I.StopDialogue(true);
            playingAudio.Clear();
        }

        public IAudioSource PlayMusic(AudioClip clip)
        {
            StopMusic();

            if (musicGroup == null)
            {
                musicGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Music);
                musicGroup.mixerGroup = AudioManager.I.musicGroup;
            }

            var source = musicGroup.Play(clip);

            return new SampleAudioSource(source, musicGroup, this);
        }

        public IAudioSource PlaySound(AudioClip clip)
        {
            if (sfxGroup == null)
            {
                sfxGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.FX);
                sfxGroup.mixerGroup = AudioManager.I.sfxGroup;
            }

            var source = sfxGroup.Play(clip);

            return new SampleAudioSource(source, sfxGroup, this);
        }

        public void Update()
        {
            for (int i = 0; i < playingAudio.Count; ++i)
            {
                if (playingAudio[i].Update())
                {
                    // could be collected
                    playingAudio.RemoveAt(i--);
                }
            }
        }

        public void OnAudioStarted(SampleAudioSource source)
        {
            if (!playingAudio.Contains(source))
                playingAudio.Add(source);
        }
    }
}
