

namespace EA4S
{
    public class SampleGameContext : IGameContext
    {
        IAudioManager audioManager = new SampleAudioManager();
        IInputManager inputManager = new SampleInputManager();

        ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();
        IStarsWidget starsWidget = new SampleStarsWidget();
        IPopupWidget questionWidget = new SamplePopupWidget();

        public IAudioManager GetAudioManager()
        {
            return audioManager;
        }
        
        public IInputManager GetInputManager()
        {
            return inputManager;
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
            inputManager.Reset();
        }
    }
}
