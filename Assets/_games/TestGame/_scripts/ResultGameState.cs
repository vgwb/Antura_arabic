namespace EA4S.Test {
    public class ResultGameState : IGameState
    {
        TestGame game;

        float timer = 4;
        public ResultGameState(TestGame game)
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
