
namespace EA4S
{
    public class SampleGameContext : IGameContext
    {
        IAudioManager audioManager = new SampleAudioManager();
        IInputManager inputManager = new SampleInputManager();
        ILogManager logManager = new MinigameLogManager();

        ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();
        IStarsWidget starsWidget = new SampleStarsWidget();
        IPopupWidget questionWidget = new SamplePopupWidget();
        ICheckmarkWidget checkmarkWidget = new SampleCheckmarkWidget();
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
