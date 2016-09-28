namespace EA4S.Tobogan
{
    public class ToboganPlayState : IGameState
    {
        ToboganGame game;
        
        public ToboganPlayState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            var nextQuestion = ToboganConfiguration.Instance.PipeQuestions.GetNextQuestion();

            game.Context.GetPopupWidget().Show(null, TextID.ASSESSMENT_RESULT_RETRY, (WordData)nextQuestion.GetQuestion());
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