namespace EA4S.Template
{
    public class QuestionGameState : IGameState
    {
        TemplateGame game;
        
        public QuestionGameState(TemplateGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetPopupWidget().Show(OnQuestionCompleted, TextID.ASSESSMENT_RESULT_GOOD, true);
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
