using System;
using DG.DeAudio;
using UnityEngine;

namespace EA4S
{
    public class SampleAudioManager : IAudioManager
    {
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

            return new SampleAudioSource(source, wordsLettersGroup);

        }

        public IAudioSource PlayLetter(LL_LetterData letterId)
        {
            return PlayLetterData(letterId);
        }

        public IAudioSource PlayWord(LL_WordData wordId)
        {
            return PlayLetterData(wordId);
        }

        public void PlayText(TextID text)
        {
            PlayDialogue(text, null);
        }

        public void PlayDialogue(TextID text, System.Action callback = null)
        {
            if (callback == null)
                AudioManager.I.PlayDialog(text.ToString());
            else
                AudioManager.I.PlayDialog(text.ToString(), callback);
        }

        public void PlayMusic(Music music)
        {
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

            return new SampleAudioSource(source, sfxGroup);
        }

        public UnityEngine.AudioClip GetAudioClip(Sfx sfx)
        {
            return AudioManager.I.GetAudioClip(sfx);
        }

        public void StopMusic()
        {
            AudioManager.I.StopMusic();
        }

        public void Reset()
        {
            AudioManager.I.ClearCache();
        }

        public IAudioSource PlayMusic(AudioClip clip)
        {
            if (musicGroup == null)
            {
                musicGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Music);
                musicGroup.mixerGroup = AudioManager.I.musicGroup;
            }

            var source = musicGroup.Play(clip);

            return new SampleAudioSource(source, musicGroup);
        }

        public IAudioSource PlaySound(AudioClip clip)
        {
            if (sfxGroup == null)
            {
                sfxGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.FX);
                sfxGroup.mixerGroup = AudioManager.I.sfxGroup;
            }

            var source = sfxGroup.Play(clip);

            return new SampleAudioSource(source, sfxGroup);
        }
    }
}
