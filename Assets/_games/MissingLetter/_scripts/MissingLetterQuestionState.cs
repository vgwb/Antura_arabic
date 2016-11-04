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
            game.Context.GetPopupWidget().Show(OnQuestionCompleted, TextID.ASSESSMENT_RESULT_GOOD, true);

            // Show questions description
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show(OnPopupCloseRequested, "MissingLetterGame description", true);
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
