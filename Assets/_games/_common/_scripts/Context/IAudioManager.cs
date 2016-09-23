// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>
namespace EA4S
{
    public interface IAudioManager
    {
        bool MusicEnabled { get; }
        void ToggleMusic();

        IAudioSource PlaySound(Sfx sfx);

        void PlayMusic(Music music);
        void StopMusic();
    }
}
