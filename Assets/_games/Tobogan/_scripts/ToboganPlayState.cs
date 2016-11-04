using System;

namespace EA4S.Tobogan
{
    public class ToboganPlayState : IGameState
    {
        CountdownTimer gameTime = new CountdownTimer(90.0f);
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
            game.questionsManager.StartNewQuestion();

            game.isTimesUp = false;
            game.ResetScore();
            
            // Reset game timer
            gameTime.Reset();
            gameTime.Start();

            game.Context.GetOverlayWidget().SetClockDuration(gameTime.Duration);
            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);


            hurryUpSfx = false;

            game.questionsManager.Enabled = true;
        }


        public void ExitState()
        {
            game.questionsManager.Enabled = false;

            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();
            
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

            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);

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
            game.Context.GetOverlayWidget().OnClockCompleted();
            game.SetCurrentState(game.ResultState);
        }
    }
}