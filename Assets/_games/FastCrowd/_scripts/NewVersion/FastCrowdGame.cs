using TMPro;
using UnityEngine;

namespace EA4S.FastCrowd
{
    public class FastCrowdGame : MiniGame
    {
        public QuestionManager QuestionManager;

        public TextMeshProUGUI timerText;

        public IQuestionPack CurrentQuestion { get; set; }
        public int CurrentScore { get; private set; }

        [HideInInspector]
        public bool isTimesUp;

        const int STARS_1_THRESHOLD = 3;
        const int STARS_2_THRESHOLD = 6;
        const int STARS_3_THRESHOLD = 9;

        public int CurrentStars
        {
            get
            {
                if (CurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        public FastCrowdIntroductionState IntroductionState { get; private set; }
        public FastCrowdQuestionState QuestionState { get; private set; }
        public FastCrowdPlayState PlayState { get; private set; }
        public FastCrowdResultState ResultState { get; private set; }

        public void ResetScore()
        {
            CurrentScore = 0;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return FastCrowdConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            float difficulty = FastCrowdConfiguration.Instance.Difficulty;

            IntroductionState = new FastCrowdIntroductionState(this);
            QuestionState = new FastCrowdQuestionState(this);
            PlayState = new FastCrowdPlayState(this);
            ResultState = new FastCrowdResultState(this);
            
            timerText.gameObject.SetActive(false);

            Physics.gravity = new Vector3(0, -10, 0);
        }
    }
}