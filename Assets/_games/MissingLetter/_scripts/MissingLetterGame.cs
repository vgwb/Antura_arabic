using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TMPro;
using UnityEngine;

namespace EA4S.MissingLetter
{
    [System.Serializable]
    public struct LLOffset
    {
        public float fINOffset;
        public float fOUTOffset;
        public float fHeightOffset;
    }

    [System.Serializable]
    public struct DifficultyConfig
    {
        public int iRoundNumberMin;
        public int iRoundNumberMax;

        public float fGameTimeMin;
        public float fGameTimeMax;

        public int iAnswerLettersNumberMin;
        public int iAnswerLettersNumberMax;
        public float fLLTotalWidth;
        public float fLLWidth;
        public float fLLMaxDistance;

        public int iAnturaTriggersNumberMin;
        public int iAnturaTriggersNumberMax;
        public float fAnturaTriggersMinoffset;

        public float fRoundNumberThresold;
        public float fGameTimeThresold;
        public float fAnswerLettersNumberThresold;
        public float fAnturaTriggersNumbersThresold;
    }

    public class MissingLetterGame : MiniGame
    {
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

        #region PRIVATE_FUNCTION
        private void CalculateDifficulty() {
            float _diff = MissingLetterConfiguration.Instance.Difficulty;

            //At least, they are all sets to the minimun
            m_iRoundsLimit = m_sDifficultyConfig.iRoundNumberMin;
            m_iNumberOfPossibleAnswers = m_sDifficultyConfig.iAnswerLettersNumberMin;
            m_fGameTime = m_sDifficultyConfig.fGameTimeMin;
            m_iAnturaTriggersNumber = m_sDifficultyConfig.iAnturaTriggersNumberMin;


            //linear calc of numbers of round (after the thresold)
            if (_diff >= m_sDifficultyConfig.fRoundNumberThresold) {
                m_iRoundsLimit = (int)Mathf.Lerp(
                    m_sDifficultyConfig.iRoundNumberMin, 
                    m_sDifficultyConfig.iRoundNumberMax, 
                    Mathf.Lerp(0.0f, 1.0f, _diff - m_sDifficultyConfig.fRoundNumberThresold)
                    );
            }

            //linear calc of game time (after the thresold)
            if (_diff >= m_sDifficultyConfig.fGameTimeThresold) {
                m_fGameTime = Mathf.Lerp(
                    m_sDifficultyConfig.fGameTimeMax,
                    m_sDifficultyConfig.fGameTimeMin,
                    Mathf.Lerp(0.0f, 1.0f, _diff - m_sDifficultyConfig.fGameTimeThresold)
                    );
            }

            //linear calc of possible answers (after the thresold)
            if (_diff >= m_sDifficultyConfig.fAnswerLettersNumberThresold) {
                m_iNumberOfPossibleAnswers = (int)Mathf.Lerp(
                    m_sDifficultyConfig.iAnswerLettersNumberMin, 
                    m_sDifficultyConfig.iAnswerLettersNumberMax,
                    Mathf.Lerp(0.0f, 1.0f, _diff - m_sDifficultyConfig.fAnswerLettersNumberThresold)
                    );
            }

            //linear calc of numbers of times that antura enter (after the thresold)
            if (_diff >= m_sDifficultyConfig.fAnturaTriggersNumbersThresold) {
                m_iAnturaTriggersNumber = (int)Mathf.Lerp(
                    m_sDifficultyConfig.iAnturaTriggersNumberMin,
                    m_sDifficultyConfig.iAnturaTriggersNumberMax, 
                    Mathf.Lerp(0.0f, 1.0f, _diff - m_sDifficultyConfig.fAnturaTriggersNumbersThresold)
                    );
            }

            //Calculating time entry point for Antura based off how many times it should enter
            m_afAnturaEnterTriggers = new float[m_iAnturaTriggersNumber];
            for(int i=0; i< m_iAnturaTriggersNumber; ++i) {
                m_afAnturaEnterTriggers[i] = ((m_fGameTime - m_sDifficultyConfig.fAnturaTriggersMinoffset) / m_iAnturaTriggersNumber) * (m_iAnturaTriggersNumber - i);
            }

            ////Calculating space between LL bases on how many should be
            //m_fDistanceBetweenLetters = (m_sDifficultyConfig.fLLTotalWidth - (m_sDifficultyConfig.fLLWidth * m_iNumberOfPossibleAnswers)) / (m_iNumberOfPossibleAnswers - 1);
            //m_fDistanceBetweenLetters = Mathf.Clamp(m_fDistanceBetweenLetters, 0.0f, m_sDifficultyConfig.fLLMaxDistance);

            //Calculating stars thresold based on Rounds Number
            STARS_1_THRESHOLD = (int)(m_iRoundsLimit * 0.25);
            STARS_2_THRESHOLD = (int)(m_iRoundsLimit * 0.55);
            STARS_3_THRESHOLD = (int)(m_iRoundsLimit * 0.95);
        }
        #endregion

        #region VARS
        public GameObject m_oLetterPrefab;
        public GameObject m_oAntura;

        public GameObject m_oEmoticonsController;
        public MissingLetterEmoticonsMaterials m_oEmoticonsMaterials;

        public GameObject m_oParticleSystem;
        public GameObject m_oSuggestionLight;
        public Collider m_oFeedBackDisableLetters;

        public Transform m_oQuestionCamera;
        public Transform m_oAnswerCamera;

        public int m_iMaxSentenceSize;

        public float m_fAnturaAnimDuration;

        public float m_fDistanceBetweenLetters;

        public LLOffset m_sQuestionOffset;
        public LLOffset m_sAnswerOffset;

        [SerializeField]
        private DifficultyConfig m_sDifficultyConfig;

        [HideInInspector]
        public int STARS_1_THRESHOLD = 2;
        [HideInInspector]
        public int STARS_2_THRESHOLD = 5;
        [HideInInspector]
        public int STARS_3_THRESHOLD = 9;


        [HideInInspector]
        public int m_iNumberOfPossibleAnswers = 4;

        [HideInInspector]
        public float[] m_afAnturaEnterTriggers;

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

        private bool m_bInIdle { get; set; }

        public MissingLetterIntroductionState IntroductionState { get; private set; }
        public MissingLetterQuestionState QuestionState { get; private set; }
        public MissingLetterPlayState PlayState { get; private set; }
        public MissingLetterResultState ResultState { get; private set; }
        public MissingLetterTutorialState TutorialState { get; private set; }
        #endregion

    }
}
