using UnityEngine;
using EA4S.Assessment;
using TMPro;

namespace EA4S.IdentifyLetter
{
    /// <summary>
    /// This game do not have any localization peculiarity. Touch the prounounced
    /// letter to make a point, more points => more reward
    /// </summary>
    public class IdentifyLetterGame : MiniGame
    {
        [Header("Managers")]
        public QuestionController questionController;

        public IdentifyLetterIntroState IntroductionState { get; private set; }
        public IdentifyLetterQuestionState QuestionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public IdentifyLetterResultState ResultState { get; private set; }
        public ScoreCounter score { get; private set; }
        public TimeEngine Time { get; private set; }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IdentifyLetterIntroState(this);
            QuestionState = new IdentifyLetterQuestionState(this);
            PlayState = new PlayGameState(this);
            ResultState = new IdentifyLetterResultState(this);
            Time = new TimeEngine();
            score = new ScoreCounter();
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return LetterShapeConfiguration.Instance;
        }
    }
}
