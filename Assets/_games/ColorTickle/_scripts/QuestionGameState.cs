namespace EA4S.ColorTickle
{
    public class QuestionGameState : IGameState
    {
        ColorTickleGame game;
        
        public QuestionGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback(OnQuestionCompleted);
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.TutorialState);
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
