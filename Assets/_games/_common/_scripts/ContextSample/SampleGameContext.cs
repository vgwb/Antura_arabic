using EA4S.API;
using System;

namespace EA4S
{
    public class SampleGameContext : IGameContext
    {
        IAudioManager audioManager = new SampleAudioManager();
        IInputManager inputManager = new SampleInputManager();
        ILogManager logManager = new AnturaLogManager();

        ISubtitlesWidget subtitleWidget = new SampleSubtitlesWidget();
        IStarsWidget starsWidget = new SampleStarsWidget();
        IPopupWidget questionWidget = new SamplePopupWidget();
        ICheckmarkWidget checkmarkWidget = new SampleCheckmarkWidget();
        IStarsBarWidget starsBarWidget = new MinigamesStarsBarWidget();
        ITimerWidget timerWidget = new MinigamesTimerWidget();
        ILivesWidget livesWidget = new MinigamesLivesWidget();

        public IAudioManager GetAudioManager()
        {
            return audioManager;
        }
        
        public IInputManager GetInputManager()
        {
            return inputManager;
        }

        public ILogManager GetLogManager() {
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
            inputManager.Reset();
        }

        public ICheckmarkWidget GetCheckmarkWidget()
        {
            return checkmarkWidget;
        }
        
        public IStarsBarWidget GetStarsBarWidget()
        {
            return starsBarWidget;
        }

        public ITimerWidget GetTimerWidget()
        {
            return timerWidget;
        }

        public ILivesWidget GetLivesWidget()
        {
            return livesWidget;
        }
    }
}
