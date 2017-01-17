
namespace EA4S
{
    /// <summary>
    /// Provides access to core functionalities to minigames.
    /// </summary>
    // refactor: this is limited, it should have references to several more managers (e.g. Tutorial, or Teacher)
    public interface IGameContext
    {
        IAudioManager GetAudioManager();
        IInputManager GetInputManager();
        ILogManager GetLogManager();

        ISubtitlesWidget GetSubtitleWidget();
        IOverlayWidget GetOverlayWidget();
        IStarsWidget GetStarsWidget();
        IPopupWidget GetPopupWidget();
        ICheckmarkWidget GetCheckmarkWidget();

        void Reset();
    }
}