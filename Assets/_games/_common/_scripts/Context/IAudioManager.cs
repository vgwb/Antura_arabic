using UnityEngine;

namespace EA4S
{
    public interface IAudioManager
    {
        bool MusicEnabled { get; set; }

        IAudioSource PlaySound(Sfx sfx);
        IAudioSource PlaySound(AudioClip clip);

        /// <summary>
        /// Play sound for letter or word,
        /// if stopAllLetters is true, it will stop any previous letter sound.
        /// </summary>
        IAudioSource PlayLetterData(ILivingLetterData id, bool stopAllLetters = false);

        IAudioSource PlayMusic(AudioClip clip);
        //TODO: IAudioSource PlayMusic(Music music);

        void Reset();

        // TODO: To be removed in next version
        void PlayMusic(Music music);
        void StopMusic();

        void PlayDialogue(Db.LocalizationDataId text, System.Action onCompleted = null);

        void Update();
    }
}
