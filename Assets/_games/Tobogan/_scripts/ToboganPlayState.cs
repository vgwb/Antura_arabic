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

            game.Context.GetTimerWidget().SetDuration(gameTime.Duration);
            game.Context.GetTimerWidget().SetTime(gameTime.Time);
            game.Context.GetTimerWidget().Show();

            hurryUpSfx = false;

            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);

            game.questionsManager.Enabled = true;
        }


        public void ExitState()
        {
            game.questionsManager.Enabled = false;

            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

            game.Context.GetTimerWidget().Hide();
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

            game.Context.GetTimerWidget().SetTime(gameTime.Time);

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