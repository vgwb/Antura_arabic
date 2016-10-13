namespace EA4S.FastCrowd
{
    public class FastCrowdQuestionState : IGameState
    {
        FastCrowdGame game;

        public FastCrowdQuestionState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            var provider = FastCrowdConfiguration.Instance.Questions;
            game.CurrentQuestion = provider.GetNextQuestion();

            if (game.CurrentQuestion != null)
            {
                // Show question
                var popupWidget = game.Context.GetPopupWidget();
                popupWidget.Show(OnPopupCloseRequested, "", true);
            }
            else
            {
                // no more questions
                game.SetCurrentState(game.ResultState);
            }
        }

        void OnPopupCloseRequested()
        {
            if (game.GetCurrentState() == this)
                game.SetCurrentState(game.PlayState);
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