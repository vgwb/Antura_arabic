namespace EA4S.MixedLetters
{
    public class ResultGameState : IGameState
    {
        MixedLettersGame game;

        float timer = 4;
        public ResultGameState(MixedLettersGame game)
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
                game.EndGame(2, 100);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
