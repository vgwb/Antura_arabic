
namespace EA4S
{
    public interface IAudioSource
    {
        bool Loop { get; set; }
        float Pitch { get; set; }
        float Volume { get; set; }
        float Duration { get; }

        bool IsPlaying { get; }

        void Play();
        void Pause();
        void Stop();
    }
}
