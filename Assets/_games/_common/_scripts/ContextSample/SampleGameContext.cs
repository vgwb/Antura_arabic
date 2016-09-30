

namespace EA4S
{
    public class SampleGameContext : IGameContext
    {
        IAudioManager audioManager = new SampleAudioManager();

        ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();
        IStarsWidget starsWidget = new SampleStarsWidget();
        IPopupWidget questionWidget = new SamplePopupWidget();

        public IAudioManager GetAudioManager()
        {
            return audioManager;
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
    }
}
