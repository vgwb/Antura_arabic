namespace EA4S
{
    public interface ITimer
    {
        bool IsRunning { get; }
        float Time { get; }

        void Start();
        void Stop();
        void Reset();

        void Update(float delta);
    }
}
