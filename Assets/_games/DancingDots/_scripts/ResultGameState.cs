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

                if (game.currStarsNum == 0)
                    AudioManager.I.PlayDialog("Reward_0Star");
                else
                    AudioManager.I.PlayDialog("Reward_" + game.currStarsNum + "Star_" + UnityEngine.Random.Range(1, 4));

            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
