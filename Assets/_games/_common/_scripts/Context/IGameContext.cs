
namespace EA4S
{
    public interface IGameContext
    {
        IAudioManager GetAudioManager();
        IInputManager GetInputManager();
        ILogManager GetLogManager();

        ISubtitlesWidget GetSubtitleWidget();
        IStarsBarWidget GetStarsBarWidget();
        ITimerWidget GetTimerWidget();
        ILivesWidget GetLivesWidget();
        IStarsWidget GetStarsWidget();
        IPopupWidget GetPopupWidget();
        ICheckmarkWidget GetCheckmarkWidget();

        void Reset();
    }
}