using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EA4S.ColorTickle
{
    public class ColorTickleGame : MiniGame
    {
        #region PUBLIC MEMBERS
        
        public GameObject m_LetterPrefab;

        [SerializeField]
        private Canvas m_ColorsCanvas;
        [SerializeField]
        private ColorTickle_AnturaController m_AnturaController;
        [SerializeField]
        private Canvas m_EndCanvas;
        [SerializeField]
        private StarFlowers m_StarsFlowers;
        [SerializeField]
        private int m_Rounds = 3;
        [SerializeField]
        private int m_MaxLives = 3;
        [SerializeField]
        private Music m_oBackgroundMusic;

        [HideInInspector]
        public int m_Stars = 0;


        // GAME STATES
        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public TutorialGameState TutorialState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        #endregion

        #region PRIVATE MEMBERS

        GameObject[] m_MyLetters;
        GameObject m_TutorialLetter;
        IOverlayWidget m_GameUI;

        #endregion

        #region GETTER/SETTER

        public GameObject[] myLetters
        {
            get { return m_MyLetters; }
            set { m_MyLetters = value; }
        }

        public GameObject tutorialLetter
        {
            get { return m_TutorialLetter; }
            set { m_TutorialLetter = value; }
        }

        public Canvas colorsCanvas
        {
            get { return m_ColorsCanvas; }
        }

        public ColorTickle_AnturaController anturaController
        {
            get { return m_AnturaController; }
        }

        public Canvas endCanvas
        {
            get { return m_EndCanvas; }
        }

        public StarFlowers starFlowers
        {
            get { return m_StarsFlowers; }
        }

        public int lives
        {
            get { return m_MaxLives; }
        }

        public int rounds
        {
            get { return m_Rounds; }
            set { m_Rounds = value; }
        }

        public IOverlayWidget gameUI
        {
            get { return m_GameUI; }
            set { m_GameUI = value; }
        }

        public Music backgroundMusic
        {
            get { return m_oBackgroundMusic; }
            set { m_oBackgroundMusic = value; }
        }

        #endregion

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            TutorialState = new TutorialGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
            return ColorTickleConfiguration.Instance;
        }

    }
}
