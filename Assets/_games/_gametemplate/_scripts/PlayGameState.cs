using Antura.MinigamesCommon;

namespace Antura.Template
{
    /// <summary>
    /// Sample game state used by the TemplateGame. 
    /// Implements the play-state of a minigame, where actual gameplay is performed.
    /// </summary>
    public class PlayGameState : IState
    {
        TemplateGame game;

        float timer = 4;
        public PlayGameState(TemplateGame game)
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
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
