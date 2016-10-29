namespace EA4S.MixedLetters
{
    public class IntroductionGameState : IGameState
    {
        MixedLettersGame game;

        float timer = 4;
        public IntroductionGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.QuestionState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}