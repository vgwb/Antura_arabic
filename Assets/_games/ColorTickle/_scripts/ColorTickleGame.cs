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
        public LetterObjectView m_LetterPrefab;
        [SerializeField]
        private Canvas m_ColorsCanvas;
        [SerializeField]
        private int m_AnimatorTickleState = 10;
        [SerializeField]
        private int m_Lives = 3;
        [SerializeField]
        private float m_Timer = 20;
        [SerializeField]
        private float m_BrushLimitVelocity = 150.0f;
        [SerializeField]
        private AnturaController m_AnturaController;


        // GAME STATES
        public IntroductionGameState IntroductionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        //public QuestionGameState QuestionState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        #endregion

        #region PRIVATE MEMBERS

        private LetterObjectView m_MyLetter;

        #endregion

        #region GETTER/SETTER

        public LetterObjectView currentLetter
        {
            get { return m_MyLetter; }
            set { m_MyLetter = value; }
        }

        public Canvas colorsCanvas
        {
            get { return m_ColorsCanvas; }
        }

        public int animatorTickleState
        {
            get { return m_AnimatorTickleState; }
        }

        public int lives
        {
            get { return m_Lives; }
        }

        public float timer
        {
            get { return m_Timer; }
        }

        public float brushLimitVelocity
        {
            get { return m_BrushLimitVelocity; }
        }

        public AnturaController anturaController
        {
            get { return m_AnturaController; }
        }

        #endregion

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            PlayState = new PlayGameState(this);
            //QuestionState = new QuestionGameState(this);
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
