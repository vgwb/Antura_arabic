using UnityEngine;

namespace EA4S.DancingDots
{
    public class IntroductionGameState : IGameState
    {
        DancingDotsGame game;

        float timer = 0;
        public IntroductionGameState(DancingDotsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            Debug.Log("Intro");
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