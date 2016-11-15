using EA4S.Assessment;

namespace EA4S.IdentifyLetter
{
    public class IdentifyLetterIntroState : IGameState
    {
        IdentifyLetterGame game;

        float timer = 1;
        public IdentifyLetterIntroState(IdentifyLetterGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            TimeEngine.Instance.Clear();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0) {
                game.SetCurrentState(game.QuestionState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}