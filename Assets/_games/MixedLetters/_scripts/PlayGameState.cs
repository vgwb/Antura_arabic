namespace EA4S.MixedLetters
{
    public class PlayGameState : IGameState
    {
        MixedLettersGame game;

        float timer = 4;
        public PlayGameState(MixedLettersGame game)
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
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
