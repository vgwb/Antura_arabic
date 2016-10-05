namespace EA4S.Tobogan
{
    public class ToboganIntroductionState : IGameState
    {
        ToboganGame game;

        float timer = 1;
        public ToboganIntroductionState(ToboganGame game)
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
                game.SetCurrentState(game.QuestionState);
                return;
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
