using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EA4S.HideAndSeek
{
    public class HideAndSeekGame : MiniGame
    {
        public IntroductionGameState IntroductionState { get; private set; }
        public QuestionGameState QuestionState { get; private set; }
        public PlayGameState PlayState { get; private set; }
        public ResultGameState ResultState { get; private set; }

        //public HideAndSeekQuestionsManager QuestionsManager { get; private set; }

        public TextMeshProUGUI timerText;

        public HideAndSeekGameManager GameManager;

        [HideInInspector]
        public bool isTimesUp;

        public bool inGame = false;

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new IntroductionGameState(this);
            QuestionState = new QuestionGameState(this);
            PlayState = new PlayGameState(this);
            ResultState = new ResultGameState(this);
            //QuestionsManager = new HideAndSeekQuestionsManager();

            timerText.gameObject.SetActive(false);
            Debug.Log(GameManager.gameObject.name);
            
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override IGameConfiguration GetConfiguration()
        {
			return HideAndSeekConfiguration.Instance;
        }
    }
}
