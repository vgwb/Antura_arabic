using EA4S.MinigamesCommon;

namespace EA4S.Minigames.MixedLetters
{
    public class ResultGameState : IGameState
    {
        private MixedLettersGame game;

        private const float TWIRL_ANIMATION_BACK_SHOWN_DELAY = 1f;
        private const float END_RESULT_DELAY = 1f;

        private float twirlAnimationDelayTimer;
        private bool wasBackShownDuringTwirlAnimation;
        private float endResultTimer;
        private bool isGameOver;

        public ResultGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            SeparateLettersSpawnerController.instance.SetLettersNonInteractive();

            game.DisableRepeatPromptButton();

            if (game.roundNumber != 0)
            {
                MinigamesUI.Timer.Pause();
            }

            if (!game.WasLastRoundWon)
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                SeparateLettersSpawnerController.instance.ShowLoseAnimation(OnResultAnimationEnded);
            }

            else
            {
                MixedLettersConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Win);
                SeparateLettersSpawnerController.instance.ShowWinAnimation(OnVictimLLIsShowingBack, OnResultAnimationEnded);

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

            twirlAnimationDelayTimer = TWIRL_ANIMATION_BACK_SHOWN_DELAY;
            wasBackShownDuringTwirlAnimation = false;
            endResultTimer = END_RESULT_DELAY;
            isGameOver = false;
        }

        private void OnVictimLLIsShowingBack()
        {
            game.GenerateNewWord();
            wasBackShownDuringTwirlAnimation = true;
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

            else if (game.WasLastRoundWon)
            {
                if (wasBackShownDuringTwirlAnimation)
                {
                    twirlAnimationDelayTimer -= delta;

                    if (twirlAnimationDelayTimer <= 0)
                    {
                        OnResultAnimationEnded();
                    }
                }
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
