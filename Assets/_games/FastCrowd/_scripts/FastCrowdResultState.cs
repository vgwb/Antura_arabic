namespace EA4S.FastCrowd
{
    public class FastCrowdResultState : IGameState
    {
        FastCrowdGame game;

        public FastCrowdResultState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            if (game.CurrentChallenge != null)
            {
                if (!game.ShowChallengePopupWidget(true, OnPopupCloseRequested))
                    game.SetCurrentState(game.QuestionState);
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