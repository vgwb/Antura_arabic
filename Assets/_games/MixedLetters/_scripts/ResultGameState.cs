namespace EA4S.MixedLetters
{
    public class ResultGameState : IGameState
    {
        IPopupWidget popupWidget;
        MixedLettersGame game;

        public ResultGameState(MixedLettersGame game)
        {
            this.game = game;
            popupWidget = MixedLettersConfiguration.Instance.Context.GetPopupWidget();
        }

        public void EnterState()
        {
            if (PlayGameState.RoundWon)
            {
                popupWidget.Show(OnResultPressed, TextID.WELL_DONE, true);
            }
            else
            {
                popupWidget.ShowTimeUp(OnResultPressed);
            }
        }

        public void ExitState()
        {
            game.ResetScene();
        }

        public void OnResultPressed()
        {
            popupWidget.Hide();
            game.SetCurrentState(game.IntroductionState);
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
