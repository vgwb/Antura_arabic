using System;

namespace EA4S.HideAndSeek
{
    public class PlayGameState : IGameState
    {
		HideAndSeekGame game;

        CountdownTimer gameTime = new CountdownTimer(30.0f);
        IAudioSource timesUpAudioSource;

        bool hurryUpSfx;

        public PlayGameState(HideAndSeekGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;

            // have to register EndGame to the death event in game (when i do the last error this event will be triggered)
        }

        public void EnterState()
        {
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

            game.Context.GetAudioManager().StopMusic();
        }

        public void Update(float delta)
        {
            game.timerText.text = String.Format("{0:0}", gameTime.Time);

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
            EndGame();
        }

        void EndGame()
        {
            game.SetCurrentState(game.ResultState);
        }
    }
}
