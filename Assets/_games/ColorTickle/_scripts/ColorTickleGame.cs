using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EA4S.ColorTickle
{
    public class ColorTickleGame : MiniGame
    {
        public LetterObjectView m_LetterPrefab;
        public IntroductionGameState IntroductionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        //public QuestionGameState QuestionState { get; private set; }
        //public ResultGameState ResultState { get; private set; }
        
        public Canvas m_ColorsCanvas;
        //public Color m_ColorBrush;

        
        private LetterObjectView m_MyLetter;

        public LetterObjectView currentLetter
        {
            get { return m_MyLetter; }
            set { m_MyLetter = value; }
        }

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
