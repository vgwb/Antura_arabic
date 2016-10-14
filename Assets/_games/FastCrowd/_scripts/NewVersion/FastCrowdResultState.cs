namespace EA4S.FastCrowd
{
    public class FastCrowdResultState : IGameState
    {
        FastCrowdGame game;

        float timer = 2;
        public FastCrowdResultState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            if (game.CurrentChallenge != null)
            {
                game.ShowChallengePopupWidget(true, OnPopupCloseRequested);
            }
        }


        void OnPopupCloseRequested()
        {
            game.SetCurrentState(game.QuestionState);
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}