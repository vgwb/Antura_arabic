using Antura.LivingLetters;

namespace Antura.Minigames.FastCrowd
{
    public class FastCrowdQuestionState : FSM.IState
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

            if (question == null)
            {
                game.SetCurrentState(game.EndState);
                return;
            }

            // TODO: make this more robust to variations
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.LetterName
                || FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.LetterForm
                )
            {
                LL_LetterData isolated = new LL_LetterData(question.GetQuestion().Id);
                isolated.Form = Database.LetterForm.Isolated;
                game.CurrentChallenge.Add(isolated);

                LL_LetterData data = new LL_LetterData(question.GetQuestion().Id);
                foreach (var form in data.Data.GetAvailableForms())
                {
                    if (form == Database.LetterForm.Isolated)
                        continue;

                    game.CurrentChallenge.Add(new LL_LetterData(data.Data, form));
                }

                if (game.CurrentChallenge.Count < 2)
                {
                    game.SetCurrentState(this);
                    return;
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
                if (!game.ShowChallengePopupWidget(false, OnPopupCloseRequested))
                {
                    if (game.showTutorial)
                    {
                        game.SetCurrentState(game.TutorialState);
                    }
                    else
                    {
                        game.SetCurrentState(game.PlayState);
                    }
                }
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
            {
                if (game.showTutorial)
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