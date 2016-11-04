using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;
using UnityEngine;

namespace EA4S.MissingLetter
{
    public class MissingLetterGame : MiniGame
    {

        public GameObject mLetterPrefab;
        public GameObject mAnturaRef;

        public TextMeshProUGUI timerText;

        public Transform mQuestionCamera;
        public Transform mAnswerCamera;

        public float mQuestionINOffset = 20;
        public float mQuestionOUTOffset = -30;
        public float mQuestionHeightOffset = -2;

        public float mAnswerINOffset = -20;
        public float mAnswerOUTOffset = 30;
        public float mAnswerHeightOffset = -2;

        public int mNumberOfPossibleAnswers = 4;

        public float mfDistanceBetweenLetters = 8.0f;

        public float mfAnturaAnimDuration = 7.0f;
        public float[] mafAnturaEnterTriggers = { 40f, 20f }; // when remains 40 and 20 seconds left
        public int miAnturaTriggersIndex = 0;

        [HideInInspector]
        public RoundManager m_RoundManager;

        [HideInInspector]
        public bool mIsTimesUp;

        public int mCurrentScore { get; private set; }


        public int mCurrentRound { get; private set; }
        public int mRoundsLimit;

        //change value for missingletter game
        const int STARS_1_THRESHOLD = 2;
        const int STARS_2_THRESHOLD = 5;
        const int STARS_3_THRESHOLD = 9;

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
        public MissingLetterTutorialState TutorialState { get; private set; }

        protected override void OnInitialize(IGameContext context)
        {
            mCurrentRound = 0;

            m_RoundManager = new RoundManager(this);
            m_RoundManager.Initialize();

            IntroductionState = new MissingLetterIntroductionState(this);
            QuestionState = new MissingLetterQuestionState(this);
            PlayState = new MissingLetterPlayState(this);
            ResultState = new MissingLetterResultState(this);
            TutorialState = new MissingLetterTutorialState(this);
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
            Context.GetCheckmarkWidget().Show(result);
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
