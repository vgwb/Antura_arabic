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
            game.Context.GetQuestionWidget().Show("bla bla", game.WordProvider.GetNextWord(), OnQuestionCompleted);
        }

        public void ExitState()
        {
            game.Context.GetQuestionWidget().Hide();
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
