using UnityEngine;

namespace EA4S.MixedLetters
{
    public class PlayGameState : IGameState
    {
        public static bool RoundWon = false;
        MixedLettersGame game;

        

        bool timerWarningSfxPlayed = false;
        public PlayGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timerWarningSfxPlayed = false;
            RoundWon = false;
            game.OnRoundStarted();

            MinigamesUI.Timer.Play();
        }

        public void ExitState()
        {

        }

        public void Update(float delta)
        {
            if (MinigamesUI.Timer.Elapsed >= MinigamesUI.Timer.Duration || RoundWon)
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
