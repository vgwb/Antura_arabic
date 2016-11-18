namespace EA4S.MissingLetter
{
    public class MissingLetterIntroductionState : IGameState
    {
        MissingLetterGame game;

        float timer = 1;
        public MissingLetterIntroductionState(MissingLetterGame game)
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
                game.SetCurrentState(game.TutorialState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}