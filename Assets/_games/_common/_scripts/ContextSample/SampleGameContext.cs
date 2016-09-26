// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>

namespace EA4S
{
    public class SampleGameContext : IGameContext
    {
        IAudioManager audioManager = new SampleAudioManager();

        ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();
        IStarsWidget starsWidget = new SampleStarsWidget();
        IQuestionWidget questionWidget = new SampleQuestionWidget();

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

        public IQuestionWidget GetQuestionWidget()
        {
            return questionWidget;
        }
    }
}
