namespace EA4S
{
    public interface IAudioManager
    {
        bool MusicEnabled { get; set; }

        IAudioSource PlaySound(Sfx sfx);

        /// <summary>
        /// Play sound for letter or word,
        /// if stopAllLetters is true, it will stop any previous letter sound.
        /// </summary>
        IAudioSource PlayLetterData(ILivingLetterData id, bool stopAllLetters = false);

        void PlayMusic(Music music);
        void StopMusic();

        void PlayDialogue(TextID text, System.Action onCompleted = null);
        
        //UnityEngine.AudioClip GetAudioClip(Sfx sfx);

        [System.Obsolete("Use PlaySound", false)]
        IAudioSource PlayLetter(LL_LetterData letterId);

        [System.Obsolete("Use PlaySound", false)]
        IAudioSource PlayWord(LL_WordData wordId);
        
        [System.Obsolete("Use PlaySound", false)]
        void PlayText(TextID text);
    }
}
