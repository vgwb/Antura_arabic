using UnityEngine;

namespace EA4S.Egg
{
    /*
     * 	
        livilli di difficoltà
        0	numero tasti lettera 3
		1	numero tasti lettera 4, sequenza
		2	numero tasti lettera 5, sequenza, antura
		3	numero tasti lettera 6, sequenza, antura, tasti non illuminati
		4	numero tasti lettera 7, sequenza, antura + 1, tasti non illuminati
		5	numero tasti lettara 8, sequenza, antura + 2, tasti non illuminati
     * 
     */

    public class EggGame : MiniGame
    {
        public EggBox eggBox;
        public EggController eggController;
        public EggButtonsBox eggButtonBox;
        public GameObject eggButtonPrefab;
        public AnturaEggController antura;
        public GameObject letterObjectPrefab;
        public EggRunLettersBox runLettersBox;
        public GameObject anturaPrefab;

        public const int numberOfStage = 4;
        public int currentStage { get; set; }

        public int correctStages { get; set; }

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

        public int gameDifficulty
        {
            get
            {
                if(EggConfiguration.Instance.Difficulty < 0.17f)
                {
                    return 0;
                }
                else if(EggConfiguration.Instance.Difficulty < 0.34f)
                {
                    return 1;
                }
                else if (EggConfiguration.Instance.Difficulty < 0.51f)
                {
                    return 2;
                }
                else if (EggConfiguration.Instance.Difficulty < 0.68f)
                {
                    return 3;
                }
                else if (EggConfiguration.Instance.Difficulty < 0.85f)
                {
                    return 4;
                } 

                return 5;
            }
        }

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

            eggController.Initialize(letterObjectPrefab, eggBox.GetEggLocalPositions());
            eggButtonBox.Initialize(eggButtonPrefab, context.GetAudioManager(), PlayState.OnEggButtonPressed);
            runLettersBox.Initialize(letterObjectPrefab);
            antura.Initialize(anturaPrefab);
        }
    }

}