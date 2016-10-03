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
            if (game.CurrentScoreRecord == 0)
            {
                game.feedbackGraphics.ShowPoorPlayerPerformanceFeedback();
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
                game.EndGame(game.CurrentStars, game.CurrentScoreRecord);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
