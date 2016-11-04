using System;

namespace EA4S.HideAndSeek
{
    public class PlayGameState : IGameState
    {
		HideAndSeekGame game;

        public CountdownTimer gameTime = new CountdownTimer(60.0f);
        IAudioSource timesUpAudioSource;

        bool hurryUpSfx;

        public PlayGameState(HideAndSeekGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            // Reset game timer
            gameTime.Reset();
            game.ResetScore();
            // game.GameManager.LifeObj.SetActive(true);

            //game.timerText.gameObject.SetActive(true);
            // game.timerText.text = "";
            hurryUpSfx = false;

            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);

            game.Context.GetOverlayWidget().SetClockDuration(gameTime.Duration);
            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);

            game.Context.GetOverlayWidget().SetMaxLives(3); 

            game.inGame = true;
            game.GameManager.SetTime();
        }

        public void ExitState()
        {
            if (timesUpAudioSource != null)
                timesUpAudioSource.Stop();

            //game.timerText.gameObject.SetActive(false);
            gameTime.Stop();

            //game.GameManager.LifeObj.SetActive(false);

            game.Context.GetAudioManager().StopMusic();

            game.inGame = false;
            game.GameManager.enabled = false;
        }

        public void Update(float delta)
        {
            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);
            //game.timerText.text = String.Format("{0:0}", gameTime.Time);

            if (!hurryUpSfx)
            {
                if (gameTime.Time < 4f)
                {
                    hurryUpSfx = true;

                    timesUpAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
                }
            }

            gameTime.Update(delta);
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
