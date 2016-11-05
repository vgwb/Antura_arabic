namespace EA4S.SickLetters
{
    public class ResultGameState : IGameState
    {
        SickLettersGame game;

        float timer = 0;
        public ResultGameState(SickLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            if (game.scale.counter < game.targetScale)
            {
                game.failure();
                timer = 10;
            }
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.EndGame(game.successRoundsCount/2, game.scale.counter);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
