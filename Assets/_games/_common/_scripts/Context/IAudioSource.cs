
namespace EA4S
{
    public interface IAudioSource
    {
        bool IsPlaying { get; }

        void Stop();
    }
}
