using DG.DeAudio;
using UnityEngine;

namespace EA4S
{
    public class SampleAudioManager : IAudioManager
    {
        DeAudioGroup wordsLettersGroup;
        DeAudioGroup sfxGroup;

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

        public IAudioSource PlayLetterData(ILivingLetterData id, bool stopAllLetters = false)
        {
            AudioClip clip = AudioManager.I.GetAudioClip(id);

            if (sfxGroup == null)
                sfxGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.Custom0);

            if (stopAllLetters)
                sfxGroup.Stop();

            var source = sfxGroup.Play(clip);

            return new SampleAudioSource(source, sfxGroup);

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
            AudioManager.I.PlayMusic(music);
        }

        public IAudioSource PlaySound(Sfx sfx)
        {
            AudioClip clip = AudioManager.I.GetAudioClip(sfx);

            if (sfxGroup == null)
                sfxGroup = DeAudioManager.GetAudioGroup(DeAudioGroupId.FX);

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
