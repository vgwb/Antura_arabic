using EA4S.MinigamesCommon;

namespace EA4S.Minigames.MixedLetters
{
    public class PlayGameState : IGameState
    {
        MixedLettersGame game;
        
        bool timerWarningSfxPlayed = false;
        public PlayGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timerWarningSfxPlayed = false;
            game.lastRoundWon = false;
            game.OnRoundStarted();

            game.EnableRepeatPromptButton();

            MinigamesUI.Timer.Play();
        }

        public void ExitState()
        {

        }

        public void Update(float delta)
        {
            if (MinigamesUI.Timer.Elapsed >= MinigamesUI.Timer.Duration || game.lastRoundWon)
            {
                game.SetCurrentState(game.ResultState);
            }

            else
            {
                if (MinigamesUI.Timer.Duration - MinigamesUI.Timer.Elapsed < 5 && !timerWarningSfxPlayed)
                {
                    MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.DangerClockLong);
                    timerWarningSfxPlayed = true;
                }
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
