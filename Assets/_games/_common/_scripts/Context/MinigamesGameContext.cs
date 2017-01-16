
namespace EA4S
{
    public class MinigamesGameContext : IGameContext
    {
        IAudioManager audioManager = new MinigamesAudioManager();
        IInputManager inputManager = new MinigamesInputManager();
        ILogManager logManager = new MinigameLogManager();

        ISubtitlesWidget subtitleWidget = new MinigamesSubtitlesWidget();
        IStarsWidget starsWidget = new MinigamesStarsWidget();
        IPopupWidget questionWidget = new MinigamesPopupWidget();
        ICheckmarkWidget checkmarkWidget = new MinigamesCheckmarkWidget();
        IOverlayWidget overlayWidget = new MinigamesOverlayWidget();

        public IAudioManager GetAudioManager()
        {
            return audioManager;
        }

        public IInputManager GetInputManager()
        {
            return inputManager;
        }

        public ILogManager GetLogManager()
        {
            return logManager;
        }

        public IStarsWidget GetStarsWidget()
        {
            return starsWidget;
        }

        public ISubtitlesWidget GetSubtitleWidget()
        {
            return subtitleWidget;
        }

        public IPopupWidget GetPopupWidget()
        {
            return questionWidget;
        }

        public void Reset()
        {
            overlayWidget.Reset();
            audioManager.Reset();
            inputManager.Reset();
        }

        public ICheckmarkWidget GetCheckmarkWidget()
        {
            return checkmarkWidget;
        }

        public IOverlayWidget GetOverlayWidget()
        {
            return overlayWidget;
        }
    }
}
