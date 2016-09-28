// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>
namespace EA4S
{
    public interface IAudioSource
    {
        bool IsPlaying { get; }

        void Stop();
    }
}
