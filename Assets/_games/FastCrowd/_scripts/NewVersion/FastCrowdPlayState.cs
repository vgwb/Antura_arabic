using System;
using System.Collections.Generic;

namespace EA4S.FastCrowd
{
    public class FastCrowdPlayState : IGameState
    {
        CountdownTimer gameTime;
        FastCrowdGame game;

        float anturaTimer;
        bool isAnturaRunning = false;

        public FastCrowdPlayState(FastCrowdGame game)
        {
            this.game = game;

            gameTime = new CountdownTimer(FastCrowdConfiguration.Instance.PlayTime);
            gameTime.onTimesUp += OnTimesUp;

            gameTime.Reset();
        }

        public void EnterState()
        {
            game.QuestionManager.OnCompleted += OnQuestionCompleted;
            game.QuestionManager.OnDropped += OnAnswerDropped;

            /*
            List<ILivingLetterData> wrongAnswers = new List<ILivingLetterData>();

            for (int i = 0; 
                i < FastCrowdConfiguration.Instance.MaxNumbOfWrongLettersNoise && 
                i < game.QuestionNumber &&
                i < game.NoiseData.Count; i++)
            {
                wrongAnswers.Add(game.NoiseData[i]);
            }
            */


            if (game.CurrentChallenge != null)
                game.QuestionManager.StartQuestion(game.CurrentChallenge, game.NoiseData);
            else
                game.QuestionManager.Clean();

            // Reset game timer
            gameTime.Start();

            game.timerText.gameObject.SetActive(true);

            StopAntura();
        }

        public void ExitState()
        {
            StopAntura();

            game.timerText.gameObject.SetActive(false);
            gameTime.Stop();
            game.QuestionManager.OnCompleted -= OnQuestionCompleted;
            game.QuestionManager.OnDropped -= OnAnswerDropped;
            game.QuestionManager.Clean();
        }
        
        void OnQuestionCompleted()
        {
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling ||
                  FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Letter)
            {
                // In spelling and letter, increment score only when the full question is completed
                game.IncrementScore();
            }

            game.SetCurrentState(game.ResultState);
        }

        void OnAnswerDropped(bool result)
        {
            game.Context.GetCheckmarkWidget().Show(result);

            if (result &&
                (FastCrowdConfiguration.Instance.Variation != FastCrowdVariation.Spelling &&
                FastCrowdConfiguration.Instance.Variation != FastCrowdVariation.Letter)
                )
            {
                // In spelling and letter, increment score only when the full question is completed
                game.IncrementScore();
            }
        }

        void StopAntura()
        {
            isAnturaRunning = false;
            game.antura.SetAnturaTime(false);
            // Schedule next exit
            anturaTimer = UnityEngine.Random.Range(20, 30);

            // TEMP
            foreach (LetterNavBehaviour item in game.QuestionManager.crowd.GetComponentsInChildren<LetterNavBehaviour>())
            {
                item.isAnturaMoment = false;
            }

            game.Context.GetAudioManager().PlayMusic(Music.Theme3);
        }

        void StartAntura()
        {
            isAnturaRunning = true;
            game.antura.SetAnturaTime(true);
            // Schedule next duration
            anturaTimer = UnityEngine.Random.Range(10, 20);

            // TEMP
            foreach (LetterNavBehaviour item in game.QuestionManager.crowd.GetComponentsInChildren<LetterNavBehaviour>())
            {
                item.isAnturaMoment = true;
            }
            
            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
        }

        public void Update(float delta)
        {
            gameTime.Update(delta);
            game.timerText.text = String.Format("{0:0}", gameTime.Time);

            anturaTimer -= delta;

            if (anturaTimer <= 0.0f)
            {
                if (isAnturaRunning)
                    StopAntura();
                else
                    StartAntura();
            }
        }

        public void UpdatePhysics(float delta)
        {
        }

        void OnTimesUp()
        {
            // Time's up!
            game.isTimesUp = true;
            game.SetCurrentState(game.EndState);
        }
    }
}