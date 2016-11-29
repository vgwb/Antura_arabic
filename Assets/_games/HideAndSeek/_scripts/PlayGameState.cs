using System;

namespace EA4S.HideAndSeek
{
    public class PlayGameState : IGameState
    {
		HideAndSeekGame game;

        public CountdownTimer gameTime = new CountdownTimer(40.0f);
        IAudioSource timesUpAudioSource;

        bool hurryUpSfx;

        const int STARS_1_THRESHOLD = 2;
        const int STARS_2_THRESHOLD = 5;
        const int STARS_3_THRESHOLD = 9;

        public PlayGameState(HideAndSeekGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            game.GameManager.enabled = true;

            game.Context.GetOverlayWidget().Initialize(true, true, true);
            game.Context.GetOverlayWidget().SetStarsThresholds(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);

            gameTime.Reset();
            game.ResetScore();
            
            hurryUpSfx = false;

            //game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
            AudioManager.I.PlayMusic(Music.MainTheme);

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

            gameTime.Stop();

            AudioManager.I.StopMusic();

            game.inGame = false;
            game.GameManager.enabled = false;
            //disable UI
            game.Context.GetOverlayWidget().Initialize(false, false, false);
        }

        public void Update(float delta)
        {
            game.Context.GetOverlayWidget().SetClockTime(gameTime.Time);

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

        public void UpdatePhysics(float delta) { }

        void OnTimesUp()
        {
            game.isTimesUp = true;
            game.SetCurrentState(game.ResultState);
        }
    }
}
