using UnityEngine;

namespace EA4S.Egg
{
    public class EggGame : MiniGame
    {
        public EggBox eggBox;
        public EggController eggController;
        public EggButtonsBox eggButtonBox;
        public GameObject eggButtonPrefab;

        public QuestionManager questionManager;

        public EggIntroductionState IntroductionState { get; private set; }
        public EggQuestionState QuestionState { get; private set; }
        public EggPlayState PlayState { get; private set; }
        public EggResultState ResultState { get; private set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return EggConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new EggIntroductionState(this);
            QuestionState = new EggQuestionState(this);
            PlayState = new EggPlayState(this);
            ResultState = new EggResultState(this);

            eggController.Initialize(eggBox.GetEggLocalPositions());
            eggButtonBox.Initialize(eggButtonPrefab);
        }
    }

}