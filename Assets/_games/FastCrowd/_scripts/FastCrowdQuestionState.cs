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
            if (game.showTutorial)
                game.QuestionManager.crowd.MaxConcurrentLetters = 2;
            else
                game.QuestionManager.crowd.MaxConcurrentLetters = UnityEngine.Mathf.RoundToInt(4 + FastCrowdConfiguration.Instance.Difficulty * 4);

            game.CurrentChallenge.Clear();
            game.NoiseData.Clear();

            var provider = FastCrowdConfiguration.Instance.Questions;
            var question = provider.GetNextQuestion();
            game.CurrentQuestion = question;

            if (question == null) {
                game.SetCurrentState(game.EndState);
                return;
            }

            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Letter) {
                LL_LetterData isolated = new LL_LetterData(question.GetQuestion().Id);
                isolated.ShowAs = Db.LetterPosition.Isolated;
                game.CurrentChallenge.Add(isolated);

                for (int i = 0; i < 3; ++i) {
                    LL_LetterData data = new LL_LetterData(question.GetQuestion().Id);

                    if (i == 0) {
                        if (data.Data.Initial_Unicode == data.Data.Isolated_Unicode)
                            continue;

                        data.ShowAs = Db.LetterPosition.Initial;
                    } else if (i == 1) {
                        if (data.Data.Medial_Unicode == data.Data.Initial_Unicode ||
                            data.Data.Medial_Unicode == data.Data.Isolated_Unicode)
                            continue;

                        data.ShowAs = Db.LetterPosition.Medial;
                    } else if (i == 2) {
                        if (data.Data.Final_Unicode == data.Data.Initial_Unicode ||
                            data.Data.Final_Unicode == data.Data.Medial_Unicode ||
                            data.Data.Final_Unicode == data.Data.Isolated_Unicode)
                            continue;

                        data.ShowAs = Db.LetterPosition.Final;
                    }

                    game.CurrentChallenge.Add(data);
                }

                if (game.CurrentChallenge.Count < 2) {
                    game.SetCurrentState(this);
                    return;
                }
            } else {
                foreach (var l in question.GetCorrectAnswers())
                    game.CurrentChallenge.Add(l);
            }

            //if (FastCrowdConfiguration.Instance.Variation != FastCrowdVariation.Alphabet)
            {
                // Add wrong data too
                if (question.GetWrongAnswers() != null)
                    foreach (var l in question.GetWrongAnswers())
                        game.NoiseData.Add(l);
            }

            ++game.QuestionNumber;

            if (game.CurrentChallenge.Count > 0) {
                // Show question
                if (!game.ShowChallengePopupWidget(false, OnPopupCloseRequested)) {
                    if (game.showTutorial)
                    {
                        game.SetCurrentState(game.TutorialState);
                    }
                    else
                    {
                        game.SetCurrentState(game.PlayState);
                    }
                }
            } else {
                // no more questions
                game.SetCurrentState(game.EndState);
            }
        }

        void OnPopupCloseRequested()
        {
            if (game.GetCurrentState() == this)
            {
                if(game.showTutorial)
                {
                    game.SetCurrentState(game.TutorialState);
                }
                else
                {
                    game.SetCurrentState(game.PlayState);
                }
            }
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