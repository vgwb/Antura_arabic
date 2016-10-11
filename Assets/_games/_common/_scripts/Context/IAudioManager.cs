namespace EA4S
{
    public interface IAudioManager
    {
        bool MusicEnabled { get; set; }

        IAudioSource PlaySound(Sfx sfx);

        void PlayMusic(Music music);
        void StopMusic();

        IAudioSource PlayLetter(string letterId);
        IAudioSource PlayWord(string wordId);
    }
}
