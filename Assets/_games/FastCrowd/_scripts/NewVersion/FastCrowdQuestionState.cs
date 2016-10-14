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

            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling)
            {
                var provider = FastCrowdConfiguration.Instance.WordsProvider;
                var question = provider.GetNextData();
                game.CurrentChallenge.Add(question);
            }
            else if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words)
            {
                for (int i = 0; i < 3; ++i)
                {
                    var provider = FastCrowdConfiguration.Instance.WordsProvider;
                    var question = provider.GetNextData();
                    game.CurrentChallenge.Add(question);
                }
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