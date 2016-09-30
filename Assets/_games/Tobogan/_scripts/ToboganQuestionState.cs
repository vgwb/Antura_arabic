namespace EA4S.Tobogan
{
    class ToboganQuestionState : IGameState
    {
        ToboganGame game;

        public ToboganQuestionState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            //var nextQuestion = ToboganConfiguration.Instance.PipeQuestions.GetNextQuestion();

            //game.Context.GetPopupWidget().Show(null, TextID.ASSESSMENT_RESULT_RETRY, (WordData)nextQuestion.GetQuestion());
        }

        public void ExitState()
        {

        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}
