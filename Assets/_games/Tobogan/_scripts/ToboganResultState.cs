namespace EA4S.Tobogan
{
    public class ToboganResultGameState : IGameState
    {
        ToboganGame game;

        float timer = 4;
        public ToboganResultGameState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            // Show some animation
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.EndGame(game.CurrentStars, game.CurrentAnswersRecord);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
