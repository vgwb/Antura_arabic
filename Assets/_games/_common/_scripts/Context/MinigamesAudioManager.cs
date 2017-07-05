using Antura.Audio;
using Antura.MinigamesAPI;
using UnityEngine;

namespace Antura.MinigamesCommon
{
    public class MinigamesAudioManager : IAudioManager
    {
        public bool MusicEnabled
        {
            get { return AudioManager.I.MusicEnabled; }
            set { AudioManager.I.MusicEnabled = value; }
        }

        public IAudioSource PlayLetterData(ILivingLetterData data, bool exclusive = true)
        {
            if (data.DataType == LivingLetterDataType.Letter) {
                return AudioManager.I.PlayLetter(new LL_LetterData(data.Id).Data, exclusive);
            } else if (data.DataType == LivingLetterDataType.Word || data.DataType == LivingLetterDataType.Image) {
                return AudioManager.I.PlayWord(new LL_WordData(data.Id).Data, exclusive);
            } else if (data.DataType == LivingLetterDataType.Phrase) {
                return AudioManager.I.PlayPhrase(new LL_PhraseData(data.Id).Data, exclusive);
            }
            return null;
        }

        public void PlayDialogue(Database.LocalizationDataId text, System.Action callback = null)
        {
            if (callback == null)
                AudioManager.I.PlayDialogue(text.ToString());
            else
                AudioManager.I.PlayDialogue(text.ToString(), callback);
        }

        public void PlayMusic(Music music)
        {
            AudioManager.I.PlayMusic(music);
        }

        public void StopMusic()
        {
            AudioManager.I.StopMusic();
        }

        public IAudioSource PlaySound(Sfx sfx)
        {
            return AudioManager.I.PlaySound(sfx);
        }

        public void StopSounds()
        {
            AudioManager.I.StopSounds();
        }

        public UnityEngine.AudioClip GetAudioClip(Sfx sfx)
        {
            return AudioManager.I.GetAudioClip(sfx);
        }

        public void Reset()
        {
            StopMusic();
            AudioManager.I.StopLettersWordsPhrases();
            AudioManager.I.ClearCache();
            AudioManager.I.StopDialogue(true);
        }

        public IAudioSource PlayMusic(AudioClip clip)
        {
            return AudioManager.I.PlayMusic(clip);
        }

        public IAudioSource PlaySound(AudioClip clip)
        {
            return AudioManager.I.PlaySound(clip);
        }

        public void Update()
        {
        }
    }
}