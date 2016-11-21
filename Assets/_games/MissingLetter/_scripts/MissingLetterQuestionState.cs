namespace EA4S.MissingLetter
{
    public class MissingLetterQuestionState : IGameState
    {
        
        public MissingLetterQuestionState(MissingLetterGame _game)
        {
            this.M_oGgame = _game;
        }

        public void EnterState()
        {
            // Show questions description
            var popupWidget = M_oGgame.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback(OnPopupCloseRequested);
            popupWidget.SetMessage("MissingLetter\n Game description", true);
        }

        void OnPopupCloseRequested()
        {
            M_oGgame.SetCurrentState(M_oGgame.TutorialState);
        }

        public void ExitState()
        {
            M_oGgame.Context.GetPopupWidget().Hide();
        }

        void OnQuestionCompleted()
        {
            M_oGgame.SetCurrentState(M_oGgame.TutorialState);
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {
        }

        MissingLetterGame M_oGgame;
    }
}
