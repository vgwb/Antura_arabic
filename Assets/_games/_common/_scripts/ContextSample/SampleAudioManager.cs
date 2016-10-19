
using System;

namespace EA4S
{
    public class SampleAudioManager : IAudioManager
    {
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

        public IAudioSource PlayLetter(LetterData letterId)
        {
            AudioManager.I.PlayLetter(letterId.Key);
            return new SampleAudioSource(null);

        }

        public IAudioSource PlayWord(WordData wordId)
        {
            AudioManager.I.PlayWord(wordId.Key);
            return new SampleAudioSource(null);
        }

        public IAudioSource PlayText(TextID text)
        {
            AudioManager.I.PlayDialog(text.ToString());
            return new SampleAudioSource(null);
        }

        public void PlayMusic(Music music)
        {
            currentMusic = music;
            AudioManager.I.PlayMusic(music);
        }

        public IAudioSource PlaySound(Sfx sfx)
        {
            // WARNING: An Audio Manager should return an Audio source, in order to manage multiple sounds using the same clip (eg. multiple bouncing balls etc.)
            // MUST BE IMPLEMENTED IN THE CURRENT AUDIO MANAGER
            AudioManager.I.PlaySfx(sfx);

            return new SampleAudioSource(sfx);
        }

        public void StopMusic()
        {
            AudioManager.I.StopMusic();
        }
    }
}
