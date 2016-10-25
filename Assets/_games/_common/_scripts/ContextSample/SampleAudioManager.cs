using DG.DeAudio;
using UnityEngine;

namespace EA4S
{
    public class SampleAudioManager : IAudioManager
    {
        DeAudioGroup sfxGroup = new DeAudioGroup(DeAudioGroupId.FX);

        Music currentMusic;
        bool musicEnabled;
        public bool MusicEnabled
        {
            get
            {
                return musicEnabled;
            }

            set
            {
                musicEnabled = value;

                if (musicEnabled)
                {
                    PlayMusic(currentMusic);
                }
                else
                {
                    StopMusic();
                }
            }
        }

        public IAudioSource PlayLetter(LL_LetterData letterId)
        {
            AudioManager.I.PlayLetter(letterId.Key);
            //return new SampleAudioSource(null);
            return null;

        }

        public IAudioSource PlayWord(LL_WordData wordId)
        {
            AudioManager.I.PlayWord(wordId.Key);
            //return new SampleAudioSource(null);
            return null;
        }

        public IAudioSource PlayText(TextID text)
        {
            AudioManager.I.PlayDialog(text.ToString());
            //return new SampleAudioSource(null);
            return null;
        }

        public void PlayMusic(Music music)
        {
            currentMusic = music;
            AudioManager.I.PlayMusic(music);
        }

        public IAudioSource PlaySound(Sfx sfx)
        {
            AudioClip clip = AudioManager.I.GetAudioClip(sfx);

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
    }
}
