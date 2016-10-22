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

            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Letter)
            {
                for (int i = 0; i < 3; ++i)
                {
                    LL_LetterData data = new LL_LetterData(question.GetQuestion().Key);

                    if (i == 0)
                        data.ShowAs = LetterDataForm.INITIAL;
                    else if (i == 1)
                        data.ShowAs = LetterDataForm.MEDIAL;
                    else if (i == 2)
                        data.ShowAs = LetterDataForm.FINAL;

                    game.CurrentChallenge.Add(data);
                }
            }
            else
            {
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