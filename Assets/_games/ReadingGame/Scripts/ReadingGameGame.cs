using System;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameGame : MiniGame // ReadingGameGameGameGameGame!
    {
        public ReadingBarSet barSet;
        public GameObject blurredText;
        public GameObject circleBox;
        public ReadingGameAntura antura;
        public ReadingRadialWidget radialWidget;

        public int CurrentScore { get; private set; }
        public int CurrentQuestionNumber { get; set; }

        [HideInInspector]
        public bool isTimesUp;

        int lives = 3;

        [HideInInspector]
        public KaraokeSong alphabetSong;
        public AudioClip alphabetSongAudio;
        public TextAsset alphabetSongSrt;

        public const int TIME_TO_ANSWER = 20;
        public const int MAX_QUESTIONS = 5;
        const int STARS_1_THRESHOLD = 8 * MAX_QUESTIONS;
        const int STARS_2_THRESHOLD = 12 * MAX_QUESTIONS;
        const int STARS_3_THRESHOLD = 15 * MAX_QUESTIONS;
        

        public int CurrentStars
        {
            get
            {
                if (CurrentScore < STARS_1_THRESHOLD)
                    return 0;
                if (CurrentScore < STARS_2_THRESHOLD)
                    return 1;
                if (CurrentScore < STARS_3_THRESHOLD)
                    return 2;
                return 3;
            }
        }
        
        public ReadingGameInitialState InitialState { get; private set; }
        public ReadingGameReadState ReadState { get; private set; }
        public ReadingGameAnswerState AnswerState { get; private set; }
        public IQuestionPack CurrentQuestion { get; set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return ReadingGameConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return InitialState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            InitialState = new ReadingGameInitialState(this);
            ReadState = new ReadingGameReadState(this);
            AnswerState = new ReadingGameAnswerState(this);

            bool isSong = (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.AlphabetSong) ;

            Context.GetOverlayWidget().Initialize(true, !isSong, !isSong);
            Context.GetOverlayWidget().SetMaxLives(lives);
            Context.GetOverlayWidget().SetLives(lives);
            Context.GetOverlayWidget().SetStarsThresholds(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.AlphabetSong)
            {
                ISongParser parser = new VttSongParser();
                alphabetSong = new KaraokeSong(parser.Parse(new MemoryStream(Encoding.UTF8.GetBytes(alphabetSongSrt.text))));
            }
        }

        public void AddScore(int score)
        {
            CurrentScore += score;

            Context.GetOverlayWidget().SetStarsScore(CurrentScore);
        }

        public bool RemoveLife()
        {
            --lives;
            Context.GetOverlayWidget().SetLives(lives);

            if (lives == 0)
            {
                EndGame(CurrentStars, CurrentScore);
                return true;
            }
            return false;
        }
    }
}