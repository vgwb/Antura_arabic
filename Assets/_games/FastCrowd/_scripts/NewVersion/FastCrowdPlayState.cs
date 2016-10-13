using System;

namespace EA4S.FastCrowd
{
    public class FastCrowdPlayState : IGameState
    {
        CountdownTimer gameTime = new CountdownTimer(90.0f);
        FastCrowdGame game;
        
        public FastCrowdPlayState(FastCrowdGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;

            gameTime.Reset();
        }

        public void EnterState()
        {
            game.QuestionManager.OnCompleted += OnQuestionCompleted;

            if (game.CurrentQuestion != null)
                game.QuestionManager.StartQuestion(game.CurrentQuestion);
            else
                game.QuestionManager.Clean();

            // Reset game timer
            gameTime.Start();

            game.timerText.gameObject.SetActive(true);
        }

        public void ExitState()
        {
            game.timerText.gameObject.SetActive(false);
            gameTime.Stop();
            game.QuestionManager.OnCompleted -= OnQuestionCompleted;
            game.QuestionManager.Clean();
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.QuestionState);
        }

        public void Update(float delta)
        {
            gameTime.Update(delta);
            game.timerText.text = String.Format("{0:0}", gameTime.Time);
        }

        public void UpdatePhysics(float delta)
        {
        }

        void OnTimesUp()
        {
            // Time's up!
            game.isTimesUp = true;
            game.SetCurrentState(game.ResultState);
        }
    }
}