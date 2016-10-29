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

        // GAME STATES
        public IntroductionGameState IntroductionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        //public QuestionGameState QuestionState { get; private set; }
        //public ResultGameState ResultState { get; private set; }

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

        #endregion

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            PlayState = new PlayGameState(this);
            //QuestionState = new QuestionGameState(this);
            //ResultState = new ResultGameState(this);
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
