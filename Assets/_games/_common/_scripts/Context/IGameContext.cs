
namespace EA4S
{
    public interface IGameContext
    {
        IAudioManager GetAudioManager();
        IInputManager GetInputManager();
        ILogManager GetLogManager();

        ISubtitlesWidget GetSubtitleWidget();
        IStarsWidget GetStarsWidget();
        IPopupWidget GetPopupWidget();
        ICheckmarkWidget GetCheckmarkWidget();

        void Reset();
    }
}