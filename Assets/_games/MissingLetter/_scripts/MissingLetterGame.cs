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
        public GameObject mFinger;

        public Transform mQuestionCamera;
        public Transform mAnswerCamera;

        public float mQuestionINOffset = 20;
        public float mQuestionOUTOffset = -30;
        public float mQuestionHeightOffset = -2;

        public float mAnswerINOffset = -20;
        public float mAnswerOUTOffset = 30;
        public float mAnswerHeightOffset = -2;

        public float mfAnturaAnimDuration = 7.0f;

        // START Difficulty CONFIG
        public int m_iRoundNumberMin = 4;
        public int m_iRoundNumberMax = 20;

        public float m_fGameTimeMin = 30.0f;
        public float m_fGameTimeMax = 60.0f;

        public int m_iAnswerLettersNumberMin = 2;
        public int m_iAnswerLettersNumberMax = 6;
        public float m_fLLTotalWidth = 30.0f;
        public float m_fLLWidth = 2.0f;
        public float m_fLLMaxDistance = 10.0f;

        public int m_iAnturaTriggersNumberMin = 1;
        public int m_iAnturaTriggersNumberMax = 4;
        public float m_fAnturaTriggersMinoffset = 10.0f;

        public float m_fRoundNumberThresold = 0.0f;
        public float m_fGameTimeThresold = 0.25f;
        public float m_fAnswerLettersNumberThresold = 0.0f;
        public float m_fAnturaTriggersNumbersThresold = 0.33f;

        public int STARS_1_THRESHOLD = 2;
        public int STARS_2_THRESHOLD = 5;
        public int STARS_3_THRESHOLD = 9;
        //END Difficulty CONFIG


        [HideInInspector]
        public int m_iNumberOfPossibleAnswers = 4;

        public float m_fDistanceBetweenLetters = 8.0f;

        [HideInInspector]
        public float[] mafAnturaEnterTriggers; // when remains 40 and 20 seconds left

        [HideInInspector]
        public int m_iAnturaTriggersNumber;

        [HideInInspector]
        public int miAnturaTriggersIndex = 0;

        [HideInInspector]
        public RoundManager m_RoundManager;

        [HideInInspector]
        public bool mIsTimesUp;

        [HideInInspector]
        public int mCurrentScore { get; private set; }

        [HideInInspector]
        public int mCurrentRound { get; private set; }

        [HideInInspector]
        public int m_iRoundsLimit;

        [HideInInspector]
        public float m_fGameTime;

        [HideInInspector]
        public bool m_bInIdle;

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

            CalculateDifficulty();

            mCurrentRound = 0;

            m_RoundManager = new RoundManager(this);
            m_RoundManager.Initialize();

            IntroductionState = new MissingLetterIntroductionState(this);
            QuestionState = new MissingLetterQuestionState(this);
            PlayState = new MissingLetterPlayState(this);
            ResultState = new MissingLetterResultState(this);
            TutorialState = new MissingLetterTutorialState(this);

            Context.GetOverlayWidget().Initialize(false, false, false);

			m_bInIdle = true;

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

            if (mCurrentRound >= m_iRoundsLimit)
            {
                this.SetCurrentState(ResultState);
            }

            if (result)
            {
                ++mCurrentScore;
            }

            Context.GetOverlayWidget().SetStarsScore(mCurrentScore);
        }

        new void OnDisable()
        {
            base.OnDisable();
            //restore the removed letter
            ((MissingLetterQuestionProvider)MissingLetterConfiguration.Instance.PipeQuestions).Restore();
		}

        public void SetInIdle(bool _idle) {
            m_bInIdle = _idle;
        }


        private void CalculateDifficulty() {
            float _diff = MissingLetterConfiguration.Instance.Difficulty;

            //At least, they are all sets to the minimun
            m_iRoundsLimit = m_iRoundNumberMin;
            m_iNumberOfPossibleAnswers = m_iAnswerLettersNumberMin;
            m_fGameTime = m_fGameTimeMin;
            m_iAnturaTriggersNumber = m_iAnturaTriggersNumberMin;


            //linear calc of numbers of round (after the thresold)
            if (_diff >= m_fRoundNumberThresold) {
                m_iRoundsLimit = (int)Mathf.Lerp(m_iRoundNumberMin, m_iRoundNumberMax, Mathf.Lerp(0.0f, 1.0f, _diff - m_fRoundNumberThresold));
            }

            //linear calc of game time (after the thresold)
            if (_diff >= m_fGameTimeThresold) {
                m_fGameTime = Mathf.Lerp(m_fGameTimeMax, m_fGameTimeMin, Mathf.Lerp(0.0f, 1.0f, _diff - m_fGameTimeThresold));
            }

            //linear calc of possible answers (after the thresold)
            if (_diff >= m_fAnswerLettersNumberThresold) {
                m_iNumberOfPossibleAnswers = (int)Mathf.Lerp(m_iAnswerLettersNumberMin, m_iAnswerLettersNumberMax, Mathf.Lerp(0.0f, 1.0f, _diff - m_fAnswerLettersNumberThresold));
            }

            //linear calc of numbers of times that antura enter (after the thresold)
            if (_diff >= m_fAnturaTriggersNumbersThresold) {
                m_iAnturaTriggersNumber = (int)Mathf.Lerp(m_iAnturaTriggersNumberMin, m_iAnturaTriggersNumberMax, Mathf.Lerp(0.0f, 1.0f, _diff - m_fAnturaTriggersNumbersThresold));
            }

            //Calculating time entry point for Antura based off how many times it should enter
            mafAnturaEnterTriggers = new float[m_iAnturaTriggersNumber];
            for(int i=0; i< m_iAnturaTriggersNumber; ++i) {
                mafAnturaEnterTriggers[i] = ((m_fGameTime - m_fAnturaTriggersMinoffset) / m_iAnturaTriggersNumber) * (m_iAnturaTriggersNumber - i);
            }

            //Calculating space between LL bases on how many should be
            m_fDistanceBetweenLetters = (m_fLLTotalWidth - (m_fLLWidth * m_iNumberOfPossibleAnswers)) / (m_iNumberOfPossibleAnswers - 1);
            m_fDistanceBetweenLetters = Mathf.Clamp(m_fDistanceBetweenLetters, 0.0f, m_fLLMaxDistance);

            //Calculating stars thresold based on Rounds Number
            STARS_1_THRESHOLD = (int)(m_iRoundsLimit * 0.25);
            STARS_2_THRESHOLD = (int)(m_iRoundsLimit * 0.55);
            STARS_3_THRESHOLD = (int)(m_iRoundsLimit * 0.95);
        }

    }
}
