using EA4S.MinigamesCommon;
using EA4S.UI;

namespace EA4S.Minigames.MixedLetters
{
    public class PlayGameState : IState
    {
        MixedLettersGame game;
        
        public PlayGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.OnRoundStarted();

            game.EnableRepeatPromptButton();

            MinigamesUI.Timer.Play();
        }

        public void ExitState()
        {

        }

        public void Update(float delta)
        {
            if (MinigamesUI.Timer.Elapsed >= MinigamesUI.Timer.Duration || game.WasLastRoundWon)
            {
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
