namespace EA4S.IdentifyLetter
{
    public class IdentifyLetterQuestionState : IGameState
    {
        IdentifyLetterGame game;
        
        public IdentifyLetterQuestionState( IdentifyLetterGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            // Enable popup widget
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback( OnPopupCloseRequested);

            // IdenfityLetter Question's description
            popupWidget.SetMessage(IdentifyLetterConfiguration.Instance.Description, true);
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState( game.PlayState);
        }

        void OnPopupCloseRequested()
        {
            game.SetCurrentState(game.PlayState);
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        public void Update( float delta)
        {

        }

        public void UpdatePhysics( float delta)
        {

        }
    }
}
