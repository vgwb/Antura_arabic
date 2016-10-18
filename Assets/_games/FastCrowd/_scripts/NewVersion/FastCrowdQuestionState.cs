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
            game.CurrentQuestion = null;

            switch (FastCrowdConfiguration.Instance.Variation)
            {
                case FastCrowdVariation.Spelling:
                    {
                        var provider = FastCrowdConfiguration.Instance.WordsProvider;
                        var question = provider.GetNextData();
                        game.CurrentChallenge.Add(question);
                        break;
                    }
                case FastCrowdVariation.Words:
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            var provider = FastCrowdConfiguration.Instance.WordsProvider;
                            var question = provider.GetNextData();
                            game.CurrentChallenge.Add(question);
                        }
                        break;
                    }
                case FastCrowdVariation.Counting:
                case FastCrowdVariation.Alphabet:
                    {
                        // Put ALL numbers or alphabet letters!
                        ILivingLetterData data = null;
                        int maxNumbers = 100;
                        int i = 0;
                        while (i < maxNumbers && (data = FastCrowdConfiguration.Instance.WordsProvider.GetNextData()) != null)
                        {
                            ++i;
                            game.CurrentChallenge.Add(data);
                        }
                        break;
                    }
                case FastCrowdVariation.Letter:
                    {
                        var provider = FastCrowdConfiguration.Instance.QuestionProvider;
                        var question = provider.GetNextQuestion();
                        game.CurrentQuestion = question;

                        foreach (var l in question.GetCorrectAnswers())
                        game.CurrentChallenge.Add(l);
                        break;
                    }
                default:
                    break;
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