namespace EA4S.FastCrowd
{
    public class FastCrowdQuestionState : IGameState
    {
        FastCrowdGame game;

        public FastCrowdQuestionState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.CurrentChallenge.Clear();
            game.NoiseData.Clear();

            var provider = FastCrowdConfiguration.Instance.Questions;
            var question = provider.GetNextQuestion();
            game.CurrentQuestion = question;

            foreach (var l in question.GetCorrectAnswers())
                game.CurrentChallenge.Add(l);

            if (FastCrowdConfiguration.Instance.Variation != FastCrowdVariation.Alphabet)
            {
                // Add wrong data too
                foreach (var l in question.GetWrongAnswers())
                    game.NoiseData.Add(l);
            }

            ++game.QuestionNumber;

            if (game.CurrentChallenge.Count > 0)
            {
                // Show question
                game.ShowChallengePopupWidget(false, OnPopupCloseRequested);
            }
            else
            {
                // no more questions
                game.SetCurrentState(game.EndState);
            }
        }

        void OnPopupCloseRequested()
        {
            if (game.GetCurrentState() == this)
                game.SetCurrentState(game.PlayState);
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