using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// This game do not have any localization peculiarity. Touch the prounounced
    /// letter to make a point, more points => more reward
    /// </summary>
    public class AssessmentGame : MiniGame
    {
        [Header("Configuration")]
        public AssessmentCode assessmentCode;

        [Header("Prefabs")]
        public AssessmentAnturaController antura;

        public AssessmentIntroState IntroductionState { get; private set; }
        public AssessmentQuestionState QuestionState { get; private set; }
        public AssessmentGameState PlayState { get; private set; }
        public AssessmentResultState ResultState { get; private set; }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new AssessmentIntroState(this);
            QuestionState = new AssessmentQuestionState(this);
            GetConfiguration();
            PlayState = new AssessmentGameState(this);
            ResultState = new AssessmentResultState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            AssessmentConfiguration.Instance.SetupDefault( assessmentCode);

            return AssessmentConfiguration.Instance;
        }
    }
}

