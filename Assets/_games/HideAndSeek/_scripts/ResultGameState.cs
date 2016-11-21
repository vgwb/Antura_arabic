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
            goToEndGame = false;
            
            AudioManager.I.PlayMusic(Music.Lullaby);

           if (game.isTimesUp)
            {
                game.Context.GetPopupWidget().Hide();
                timer = 1;
                goToEndGame = true;
            }
        }

        public void ExitState()
        {
            AudioManager.I.StopMusic();
        }

        public void Update(float delta)
        {
            if (!game.isTimesUp || goToEndGame)
                timer -= delta;

            if (timer < 0)
                 game.EndGame(game.CurrentStars, game.CurrentScore);
        }

        public void UpdatePhysics(float delta) { }
    }
}
