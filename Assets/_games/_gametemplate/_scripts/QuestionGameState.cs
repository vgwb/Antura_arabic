using Antura.Minigames;

namespace Antura.Template
{
    /// <summary>
    /// Sample game state used by the TemplateGame. 
    /// Implements a phase where a question is shown to the player.
    /// </summary>
    public class QuestionGameState : IState
    {
        TemplateGame game;
        
        public QuestionGameState(TemplateGame game)
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
            game.SetCurrentState(game.PlayState);
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
