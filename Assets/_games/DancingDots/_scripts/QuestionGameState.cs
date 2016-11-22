namespace EA4S.DancingDots
{
    public class QuestionGameState : IGameState
    {
        DancingDotsGame game;
        
        public QuestionGameState(DancingDotsGame game)
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
            game.SetCurrentState(game.PlayState);
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
