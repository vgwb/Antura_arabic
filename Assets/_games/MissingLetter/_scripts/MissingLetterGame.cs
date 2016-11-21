using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;
using UnityEngine;

namespace EA4S.MissingLetter
{
    //enum game type
    public enum GameType
    {
        WORD = 0,
        SENTENCE = 1
    }

    public class MissingLetterGame : MiniGame
    {
        #region VARS
        public GameType m_eGameType;
        public GameObject m_oLetterPrefab;
        public GameObject m_oAntura;
        public GameObject m_oEmoticonsController;
        public GameObject m_oParticleSystem;
        public Collider m_oFeedBackDisableLetters;

        public Transform m_oQuestionCamera;
        public Transform m_oAnswerCamera;

        public int m_iMaxSentenceSize = 5;

        public float m_fQuestionINOffset = 20;
        public float m_fQuestionOUTOffset = -30;
        public float m_fQuestionHeightOffset = -2;

        public float m_fAnswerINOffset = -20;
        public float m_fAnswerOUTOffset = 30;
        public float m_fAnswerHeightOffset = -2;

        public float m_fAnturaAnimDuration = 7.0f;

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

        [HideInInspector]
        public int STARS_1_THRESHOLD = 2;
        [HideInInspector]
        public int STARS_2_THRESHOLD = 5;
        [HideInInspector]
        public int STARS_3_THRESHOLD = 9;
        //END Difficulty CONFIG


        [HideInInspector]
        public int m_iNumberOfPossibleAnswers = 4;

        public float m_fDistanceBetweenLetters = 8.0f;

        [HideInInspector]
        public float[] m_afAnturaEnterTriggers; // when remains 40 and 20 seconds left

        [HideInInspector]
        public int m_iAnturaTriggersNumber;

        [HideInInspector]
        public int m_iAnturaTriggersIndex = 0;

        [HideInInspector]
        public RoundManager m_oRoundManager;

        [HideInInspector]
        public bool m_bIsTimesUp;

        [HideInInspector]
        public int m_iCurrentScore { get; private set; }

        [HideInInspector]
        public int m_iCurrentRound { get; private set; }

        [HideInInspector]
        public int m_iRoundsLimit;

        [HideInInspector]
        public float m_fGameTime;

        private bool m_bInIdle { get; set; }

        public int m_iCurrentStars
        {
            get
            {
                if (m_iCurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (m_iCurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (m_iCurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }

        public MissingLetterIntroductionState IntroductionState { get; private set; }
        public MissingLetterQuestionState QuestionState { get; private set; }
        public MissingLetterPlayState PlayState { get; private set; }
        public MissingLetterResultState ResultState { get; private set; }
        public MissingLetterTutorialState TutorialState { get; private set; }
        #endregion

        #region PROTECTED_FUNCTION
        protected override void OnInitialize(IGameContext context)
        {

            CalculateDifficulty();

            m_iCurrentRound = 0;

            m_oRoundManager = new RoundManager(this);
            m_oRoundManager.Initialize();

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
        #endregion

        #region API
        public void ResetScore()
        {
            m_iCurrentScore = 0;
        }

        public void OnResult(bool _result)
        {
            Context.GetCheckmarkWidget().Show(_result);
            m_iCurrentRound++;

            if (m_iCurrentRound >= m_iRoundsLimit)
            {
                this.SetCurrentState(ResultState);
            }

            if (_result)
            {
                ++m_iCurrentScore;
            }

            Context.GetOverlayWidget().SetStarsScore(m_iCurrentScore);
        }

        public void SetInIdle(bool _idle) {
            m_oFeedBackDisableLetters.enabled = !_idle;
            m_bInIdle = _idle;
        }

        public bool IsInIdle()
        {
            return m_bInIdle;
        }
        #endregion

        #region PRIVATE_FUNCTION
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
            m_afAnturaEnterTriggers = new float[m_iAnturaTriggersNumber];
            for(int i=0; i< m_iAnturaTriggersNumber; ++i) {
                m_afAnturaEnterTriggers[i] = ((m_fGameTime - m_fAnturaTriggersMinoffset) / m_iAnturaTriggersNumber) * (m_iAnturaTriggersNumber - i);
            }

            //Calculating space between LL bases on how many should be
            m_fDistanceBetweenLetters = (m_fLLTotalWidth - (m_fLLWidth * m_iNumberOfPossibleAnswers)) / (m_iNumberOfPossibleAnswers - 1);
            m_fDistanceBetweenLetters = Mathf.Clamp(m_fDistanceBetweenLetters, 0.0f, m_fLLMaxDistance);

            //Calculating stars thresold based on Rounds Number
            STARS_1_THRESHOLD = (int)(m_iRoundsLimit * 0.25);
            STARS_2_THRESHOLD = (int)(m_iRoundsLimit * 0.55);
            STARS_3_THRESHOLD = (int)(m_iRoundsLimit * 0.95);
        }
        #endregion

    }
}
