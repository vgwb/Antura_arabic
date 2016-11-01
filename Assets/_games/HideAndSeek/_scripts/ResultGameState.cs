namespace EA4S.HideAndSeek
{
    public class ResultGameState : IGameState
    {
		HideAndSeekGame game;

        bool goToEndGame;

        float timer = 4;
		public ResultGameState(HideAndSeekGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            //timer = 4;

            goToEndGame = false;

            game.Context.GetAudioManager().PlayMusic(Music.Lullaby);

            if (game.isTimesUp)
            {
                game.Context.GetPopupWidget().ShowTimeUp(OnPopupTimeUpCloseRequested);
            }
        }

        public void ExitState()
        {
            game.Context.GetAudioManager().StopMusic();
        }

        void OnPopupTimeUpCloseRequested()
        {
            game.Context.GetPopupWidget().Hide();
            timer = 1;
            goToEndGame = true;
        }

        public void Update(float delta)
        {
            if (!game.isTimesUp || goToEndGame)
                timer -= delta;

            if (timer < 0)
            {
                game.EndGame(2, 100);
                // game.EndGame(game.CurrentStars, game.CurrentScoreRecord);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
