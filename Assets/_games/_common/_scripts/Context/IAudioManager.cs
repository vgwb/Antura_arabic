namespace EA4S
{
    public interface IAudioManager
    {
        bool MusicEnabled { get; set; }

        IAudioSource PlaySound(Sfx sfx);

        void PlayMusic(Music music);
        void StopMusic();

        IAudioSource PlayLetter(LetterData letterId);
        IAudioSource PlayWord(WordData wordId);
        IAudioSource PlayText(TextID text);
    }
}
