namespace EA4S.DancingDots
{
    public class PlayGameState : IGameState
    {
        DancingDotsGame game;

        float timer;
        public PlayGameState(DancingDotsGame game)
        {
            this.game = game;
            
        }

        public void EnterState()
        {
            game.StartRound();
            timer = game.gameDuration;
        }

        public void ExitState()
        {
            game.EndGame();
        }

        public void Update(float delta)
        {
            if (!game.isTutRound)
            {
                timer -= delta;
                game.Context.GetOverlayWidget().SetClockTime(timer);
            }
            

            if (timer < 0)
            {
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
