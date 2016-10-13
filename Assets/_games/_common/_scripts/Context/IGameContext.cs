
namespace EA4S
{
    public interface IGameContext
    {
        IAudioManager GetAudioManager();
        IInputManager GetInputManager();

        ISubtitlesWidget GetSubtitleWidget();
        IStarsWidget GetStarsWidget();
        IPopupWidget GetPopupWidget();

        void Reset();
    }
}