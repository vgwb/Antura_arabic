namespace EA4S.Tobogan
{
    public class ToboganQuestionState : IGameState
    {
        ToboganGame game;

        public ToboganQuestionState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.questionsManager.Initialize();

            // Show questions description
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show(OnPopupCloseRequested, ToboganConfiguration.Instance.PipeQuestions.GetDescription(), true);
        }

        void OnPopupCloseRequested()
        {
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
