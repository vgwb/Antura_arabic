namespace EA4S.FastCrowd
{
    public class FastCrowdResultState : IGameState
    {
        FastCrowdGame game;

        float timer = 3;
        public FastCrowdResultState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.EndGame(game.CurrentStars, game.CurrentScore);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}