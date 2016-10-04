using System;

namespace EA4S.Tobogan
{
    public class ToboganPlayState : IGameState
    {
        CountdownTimer gameTime = new CountdownTimer(99.0f);
        ToboganGame game;
        IAudioSource timesUpAudioSource;

        bool hurryUpSfx;

        public ToboganPlayState(ToboganGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            game.isTimesUp = false;
            game.ResetScore();
            
            // Reset game timer
            gameTime.Reset();
            gameTime.Start();

            game.timerText.gameObject.SetActive(true);
            game.timerText.text = "";
            hurryUpSfx = false;

            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
        }


        public void ExitState()
        {
            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

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

            if(!hurryUpSfx)
            {
                if (gameTime.Time < 4f)
                {
                    hurryUpSfx = true;

                    timesUpAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
                }
            }
            
            gameTime.Update(delta);

            game.questionsManager.Update(delta);
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