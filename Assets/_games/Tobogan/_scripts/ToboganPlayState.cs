using System;

namespace EA4S.Tobogan
{
    public class ToboganPlayState : IGameState
    {
        CountdownTimer gameTime = new CountdownTimer(99.0f);
        ToboganGame game;
        
        public ToboganPlayState(ToboganGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            game.ResetScore();
            
            // Reset game timer
            gameTime.Reset();
            gameTime.Start();

            game.timerText.gameObject.SetActive(true);
            game.timerText.text = "";

            //game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
        }


        public void ExitState()
        {
            game.timerText.gameObject.SetActive(false);
            gameTime.Stop();
            game.pipesAnswerController.HidePipes();
        }

        public void Update(float delta)
        {
            if (game.CurrentScoreRecord >= ToboganGame.MAX_ANSWERS_RECORD)
            {
                // Maximum tower height reached
                game.SetCurrentState(game.ResultState);
                return;
            }

            game.timerText.text = String.Format("{0:0}", gameTime.Time);
            gameTime.Update(delta);

            game.questionsManager.Update(delta);
        }

        public void UpdatePhysics(float delta)
        {

        }

        void OnTimesUp()
        {
            // Time's up!
            game.SetCurrentState(game.ResultState);
        }
    }
}