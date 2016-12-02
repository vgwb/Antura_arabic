using System.Linq;
using System.Collections;
using UnityEngine;

namespace EA4S.MixedLetters
{
    public class ResultGameState : IGameState
    {
        IPopupWidget popupWidget;
        MixedLettersGame game;

        float endResultTimer = 1f;
        bool isGameOver = false;

        public ResultGameState(MixedLettersGame game)
        {
            this.game = game;
            popupWidget = MixedLettersConfiguration.Instance.Context.GetPopupWidget();
        }

        public void EnterState()
        {
            SeparateLettersSpawnerController.instance.SetLettersNonInteractive();

            if (game.roundNumber != 0)
            {
                MinigamesUI.Timer.Pause();
            }

            if (!game.lastRoundWon)
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                SeparateLettersSpawnerController.instance.ShowLoseAnimation(OnResultAnimationEnded);
            }

            else
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Win);
                SeparateLettersSpawnerController.instance.ShowWinAnimation(OnResultAnimationEnded);

                if (game.numRoundsWon == 1)
                {
                    MinigamesUI.Starbar.GotoStar(0);
                }

                else if (game.numRoundsWon == 3)
                {
                    MinigamesUI.Starbar.GotoStar(1);
                }

                else if (game.numRoundsWon == 5)
                {
                    MinigamesUI.Starbar.GotoStar(2);
                }
            }
        }

        public void ExitState()
        {
            game.ResetScene();
        }

        public void OnResultAnimationEnded()
        {
            if (game.roundNumber < 5)
            {
                game.SetCurrentState(game.IntroductionState);
            }

            else
            {
                isGameOver = true;
            }
        }

        public void Update(float delta)
        {
            if (isGameOver)
            {
                endResultTimer -= delta;

                if (endResultTimer < 0)
                {
                    int numberOfStars;

                    if (game.numRoundsWon == 0)
                    {
                        numberOfStars = 0;
                    }
                    else if (game.numRoundsWon == 1 || game.numRoundsWon == 2)
                    {
                        numberOfStars = 1;
                    }
                    else if (game.numRoundsWon == 3 || game.numRoundsWon == 4)
                    {
                        numberOfStars = 2;
                    }
                    else
                    {
                        numberOfStars = 3;
                    }

                    game.EndGame(numberOfStars, 0);
                }
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
