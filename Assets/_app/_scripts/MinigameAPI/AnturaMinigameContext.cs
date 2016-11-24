using EA4S;

namespace EA4S.API
{

    /// <summary>
    /// Default Context for Antura Minigame.
    /// </summary>
    /// <seealso cref="EA4S.IGameContext" />
    public class AnturaMinigameContext : IGameContext
    {

        #region Log Manager 
        public ILogManager logManager = new LogManager().MinigameLogManager;

        public ILogManager GetLogManager()
        {
            return logManager;
        }
        #endregion

        #region AudioManager provider
        public IAudioManager audioManager = new SampleAudioManager();

        public IAudioManager GetAudioManager()
        {
            return audioManager;
        }
        #endregion

        #region InputManger provider
        public IInputManager inputManager = new SampleInputManager();

        public IInputManager GetInputManager()
        {
            return inputManager;
        }
        #endregion

        #region SubTitle provider
        public ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();

        public IStarsWidget GetStarsWidget()
        {
            return starsWidget;
        }
        #endregion

        #region StarsWidget provider
        public IStarsWidget starsWidget = new SampleStarsWidget();

        public ISubtitlesWidget GetSubtitleWidget()
        {
            return subtitleWidget;
        }
        #endregion

        #region PopupWidget provider
        public IPopupWidget questionWidget = new SamplePopupWidget();
        public IPopupWidget GetPopupWidget()
        {
            return questionWidget;
        }
        #endregion

        public void Reset()
        {
            inputManager.Reset();
            audioManager.Reset();
            overlayWidget.Reset();
        }

        #region CheckmarkWidget provider
        public ICheckmarkWidget checkmarkWidget = new SampleCheckmarkWidget();
        public ICheckmarkWidget GetCheckmarkWidget()
        {
            return checkmarkWidget;
        }
        #endregion

        #region OverlayWidget provider
        IOverlayWidget overlayWidget = new MinigamesOverlayWidget();
        public IOverlayWidget GetOverlayWidget()
        {
            return overlayWidget;
        }
        #endregion

        #region Context Presets

        public static AnturaMinigameContext Default = new AnturaMinigameContext() {
            logManager = new MinigameLogManager(),
            audioManager = new SampleAudioManager(),
            subtitleWidget = new SampleSubtitlesWidget(),
            starsWidget = new SampleStarsWidget(),
            questionWidget = new SamplePopupWidget(),
        };

        /// <summary>
        /// Example for custom context preset used for fast crowd.
        /// </summary>
        public static AnturaMinigameContext FastCrowd = new AnturaMinigameContext() {
            logManager = new MinigameLogManager(),
            audioManager = new SampleAudioManager(),
            subtitleWidget = new SampleSubtitlesWidget(),
            starsWidget = new SampleStarsWidget(),
            questionWidget = new SamplePopupWidget(),
        };

        #endregion
    }
}