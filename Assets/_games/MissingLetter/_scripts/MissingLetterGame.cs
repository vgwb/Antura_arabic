using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;
using UnityEngine;

namespace EA4S.MissingLetter
{
    public class MissingLetterGame : MiniGame
    {

        public TextMeshProUGUI timerText;

        public GameObject mLetterPrefab;

        public Transform mQuestionCamera;
        public Transform mAnswerCamera;


        public float mQuestionINOffset = 20;
        public float mQuestionOUTOffset = -30;
        public float mQuestionHeightOffset = -2;

        public float mAnswerINOffset = -20;
        public float mAnswerOUTOffset = 30;
        public float mAnswerHeightOffset = -2;

        public int mNumberOfPossibleAnswers = 4;

        public GameObject mAnturaRef;

        //[SerializeField]
        //private float m_kfDistanceBetweenLetters = 8.0f;

        [HideInInspector]
        public RoundManager m_RoundManager;

        [HideInInspector]
        public bool mIsTimesUp;

        public int mCurrentScore { get; private set; }


        public int mCurrentRound { get; private set; }
        public int mRoundsLimit;

        //change value for missingletter game
        const int STARS_1_THRESHOLD = 5;
        const int STARS_2_THRESHOLD = 8;
        const int STARS_3_THRESHOLD = 12;

        public int CurrentStars
        {
            get
            {
                if (mCurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (mCurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (mCurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        public MissingLetterIntroductionState IntroductionState { get; private set; }
        public MissingLetterQuestionState QuestionState { get; private set; }
        public MissingLetterPlayState PlayState { get; private set; }
        public MissingLetterResultState ResultState { get; private set; }

        protected override void OnInitialize(IGameContext context)
        {
            mCurrentRound = 0;
            m_RoundManager = new RoundManager(this);
            m_RoundManager.Initialize();

            IntroductionState = new MissingLetterIntroductionState(this);
            QuestionState = new MissingLetterQuestionState(this);
            PlayState = new MissingLetterPlayState(this);
            ResultState = new MissingLetterResultState(this);

        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return MissingLetterConfiguration.Instance;
        }

        public void ResetScore()
        {
            mCurrentScore = 0;
        }

        public void OnResult(bool result)
        {
            mCurrentRound++;

            if (mCurrentRound >= mRoundsLimit)
            {
                this.SetCurrentState(ResultState);
            }

            if (result)
            {
                ++mCurrentScore;
            }

        }

    }
}
