namespace EA4S.DancingDots
{
    public class ResultGameState : IGameState
    {
        DancingDotsGame game;

        float timer = 0; 
        public ResultGameState(DancingDotsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            
            //game.Context.GetStarsWidget().Hide();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {

                //game.Context.GetOverlayWidget().Hide();
                game.EndGame(game.currStarsNum, 100);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
