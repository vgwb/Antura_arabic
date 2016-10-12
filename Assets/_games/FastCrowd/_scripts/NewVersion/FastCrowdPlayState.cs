namespace EA4S.FastCrowd
{
    public class FastCrowdPlayState : IGameState
    {
        FastCrowdGame game;
        
        public FastCrowdPlayState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            if (game.CurrentQuestion != null)
                game.QuestionManager.StartQuestion(game.CurrentQuestion);
            else
                game.QuestionManager.Clean();
        }

        public void ExitState()
        {
            game.QuestionManager.Clean();
        }

        public void Update(float delta)
        {
         
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}