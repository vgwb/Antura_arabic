namespace EA4S
{
    public interface IAudioManager
    {
        bool MusicEnabled { get; set; }

        IAudioSource PlaySound(Sfx sfx);

        void PlayMusic(Music music);
        void StopMusic();

        IAudioSource PlayLetter(LL_LetterData letterId);
        IAudioSource PlayWord(LL_WordData wordId);
        IAudioSource PlayText(TextID text);

        UnityEngine.AudioClip GetAudioClip(Sfx sfx);
    }
}
