namespace EA4S.SickLetters
{
    public class ResultGameState : IGameState
    {
        SickLettersGame game;
        int stars, score;

        float timer = 4;
        public ResultGameState(SickLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            if (game.scale.counter < game.targetScale)
            {
                game.manager.failure();

                timer = 8;
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
