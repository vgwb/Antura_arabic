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
            //ColorButtonsManager colorsButtonsManager = game.colorsCanvas.GetComponentInChildren<ColorButtonsManager>();
            //colorsButtonsManager.SetButtonsOnPosition();
            game.Context.GetPopupWidget().Show(OnQuestionCompleted, TextID.ASSESSMENT_RESULT_GOOD, true);
			for (int i = 0; i < game.rounds; ++i) {
				game.myLetters[i].gameObject.SetActive (false);
			}
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.PlayState);
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
