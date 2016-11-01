using UnityEngine;

namespace EA4S.MixedLetters
{
    public class PlayGameState : IGameState
    {
        public static bool RoundWon = false;
        MixedLettersGame game;

        float timer = 4;
        bool timerWarningSfxPlayed = false;
        public PlayGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timer = 30.99f;
            timerWarningSfxPlayed = false;
            RoundWon = false;
            game.OnRoundStarted(Mathf.FloorToInt(timer));
        }

        public void ExitState()
        {
            
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0 || RoundWon)
            {
                game.SetCurrentState(game.ResultState);
            }

            else
            {
                UIController.instance.SetTimer(Mathf.FloorToInt(timer));

                if (timer < 5 && !timerWarningSfxPlayed)
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
