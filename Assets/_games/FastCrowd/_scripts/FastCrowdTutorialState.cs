namespace EA4S.FastCrowd
{
    public class FastCrowdTutorialState : IGameState
    {
        FastCrowdGame game;

        public FastCrowdTutorialState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.QuestionManager.OnCompleted += OnQuestionCompleted;
            game.QuestionManager.OnDropped += OnAnswerDropped;

            if (game.CurrentChallenge != null)
                game.QuestionManager.StartQuestion(game.CurrentChallenge, game.NoiseData);
            else
                game.QuestionManager.Clean();
        }

        public void ExitState()
        {
            game.QuestionManager.OnCompleted -= OnQuestionCompleted;
            game.QuestionManager.OnDropped -= OnAnswerDropped;
            game.QuestionManager.Clean();
        }

        void OnQuestionCompleted()
        {
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Spelling ||
                  FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Letter)
            {
                // In spelling and letter, increment score only when the full question is completed
                game.IncrementScore();
            }

            game.SetCurrentState(game.ResultState);
        }

        void OnAnswerDropped(bool result)
        {
            game.Context.GetCheckmarkWidget().Show(result);

            if (result &&
                (FastCrowdConfiguration.Instance.Variation != FastCrowdVariation.Spelling &&
                FastCrowdConfiguration.Instance.Variation != FastCrowdVariation.Letter)
                )
            {
                // In spelling and letter, increment score only when the full question is completed
                game.IncrementScore();
            }

            game.Context.GetAudioManager().PlaySound(result ? Sfx.OK : Sfx.KO);
        }

        public void Update(float delta)
        {
            
        }

        public void UpdatePhysics(float delta)
        {
            
        }
    }
}
