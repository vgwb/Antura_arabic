using UnityEngine;
using System.Collections;

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

        public AssessmentIntroState IntroductionState { get; private set; }
        public AssessmentQuestionState QuestionState { get; private set; }
        public AssessmentGameState PlayState { get; private set; }
        public AssessmentResultState ResultState { get; private set; }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new AssessmentIntroState(this);
            QuestionState = new AssessmentQuestionState(this);
            PlayState = new AssessmentGameState(this);
            ResultState = new AssessmentResultState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            if (AssessmentConfiguration.Instance.assessmentType == AssessmentCode.Unsetted)
                AssessmentConfiguration.Instance.assessmentType = assessmentCode;

            return AssessmentConfiguration.Instance;
        }
    }
}

