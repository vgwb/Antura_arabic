using Antura.MinigamesCommon;

namespace Antura.Template
{
    /// <summary>
    /// Sample game state used by the TemplateGame. 
    /// Implements a timed introduction before advancing to the next state.
    /// </summary>
    public class IntroductionGameState : IState
    {
        TemplateGame game;

        float timer = 4;
        public IntroductionGameState(TemplateGame game)
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