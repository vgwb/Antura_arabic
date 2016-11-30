using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentGame : MiniGame
    {
        [Header("Configuration")]
        public AssessmentCode assessmentCode;

        public AssessmentIntroState IntroState { get; private set; }
        public AssessmentGameState PlayState { get; private set; }
        public AssessmentResultState ResultState { get; private set; }

        protected override void OnInitialize( IGameContext context)
        {
            IntroState = new AssessmentIntroState( this);
            GetConfiguration();
            PlayState = new AssessmentGameState( this);
            ResultState = new AssessmentResultState( this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return AssessmentConfiguration.Instance;
        }
    }
}

