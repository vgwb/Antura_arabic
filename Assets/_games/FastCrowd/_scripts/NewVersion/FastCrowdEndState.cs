namespace EA4S.FastCrowd
{
    public class FastCrowdEndState : IGameState
    {
        FastCrowdGame game;

        float timer = 2;
        public FastCrowdEndState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetAudioManager().PlayMusic(Music.Relax);
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