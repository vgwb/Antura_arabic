namespace EA4S.MissingLetter
{
    public class MissingLetterQuestionState : IGameState
    {
        MissingLetterGame game;
        
        public MissingLetterQuestionState(MissingLetterGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            // Show questions description
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback(OnPopupCloseRequested);
            popupWidget.SetMessage("MissingLetter\n Game description", true);
        }

        void OnPopupCloseRequested()
        {
            game.SetCurrentState(game.TutorialState);
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
