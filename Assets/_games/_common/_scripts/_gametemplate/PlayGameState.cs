namespace EA4S.Template
{
    public class PlayGameState : IGameState
    {
        TemplateGame game;

        float timer = 4;
        public PlayGameState(TemplateGame game)
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
