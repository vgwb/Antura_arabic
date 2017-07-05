namespace Antura.MinigamesCommon
{
    /// <summary>
    /// Concrete implementation of IGameContext. Accessible to minigames.
    /// </summary>
    public class MinigamesGameContext : IGameContext
    {
        public MiniGameCode Code { get; private set; }

        IAudioManager audioManager = new MinigamesAudioManager();
        IInputManager inputManager = new MinigamesInputManager();

        ISubtitlesWidget subtitleWidget = new MinigamesSubtitlesWidget();
        IStarsWidget starsWidget = new MinigamesStarsWidget();
        IPopupWidget questionWidget = new MinigamesPopupWidget();
        ICheckmarkWidget checkmarkWidget = new MinigamesCheckmarkWidget();
        IOverlayWidget overlayWidget = new MinigamesOverlayWidget();
        ILogManager logManager;

        public MinigamesGameContext(MiniGameCode code, string sessionName)
        {
            Code = code;
            logManager = new MinigamesLogManager(code, sessionName);
        }

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
