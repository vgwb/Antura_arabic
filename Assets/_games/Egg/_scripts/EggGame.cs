using UnityEngine;

namespace EA4S.Egg
{
    public class EggGame : MiniGame
    {
        public EggBox eggBox;
        public EggController eggController;
        public EggButtonsBox eggButtonBox;
        public GameObject eggButtonPrefab;
        public AnturaEggController antura;

        public const int numberOfStage = 4;
        public int currentStage { get; set; }

        public int correctStages = 0;

        public int CurrentStars
        {
            get
            {
                if (correctStages == 0)
                    return 0;

                if (correctStages == 1)
                    return 1;

                if (correctStages == 2 || correctStages == 3)
                    return 2;

                return 3;
            }
        }

        public bool stagePositiveResult { get; set; }

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

            questionManager = new QuestionManager(this);

            currentStage = 0;
            correctStages = 0;

            eggController.Initialize(eggBox.GetEggLocalPositions());
            eggButtonBox.Initialize(eggButtonPrefab, context.GetAudioManager(), PlayState.OnEggButtonPressed);
        }
    }

}