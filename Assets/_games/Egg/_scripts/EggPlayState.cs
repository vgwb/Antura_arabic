using UnityEngine;

namespace EA4S.Egg
{
    public class EggPlayState : IGameState
    {
        EggGame game;

        int letterOnSequence;
        bool gameModeOneLetter;

        int questionProgress;
        int correctAnswers;

        float nextStateTimer;
        bool toNextState;

        float inputButtonTime = 0.3f;
        float inputButtonTimer;
        int inputButtonCount;
        int inputButtonMax = 4;
        bool progressInput;

        IAudioSource positiveAudioSource;

        public EggPlayState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            letterOnSequence = 0;

            gameModeOneLetter = !game.questionManager.IsSequence();

            questionProgress = 0;

            if (gameModeOneLetter)
            {
                correctAnswers = 3;
            }
            else
            {
                correctAnswers = game.questionManager.GetlLetterDataSequence().Count;
            }

            game.eggController.onEggCrackComplete = OnEggCrackComplete;
            game.eggController.onEggExitComplete = OnEggExitComplete;
            game.eggController.onEggPressedCallback = OnEggPressed;

            EnableAllGameplayInput();
            
            nextStateTimer = 0f;
            toNextState = false;

            inputButtonTimer = 0f;
            inputButtonCount = 0;
            progressInput = false;
        }

        public void ExitState() { }

        public void Update(float delta)
        {
            if (toNextState)
            {
                nextStateTimer -= delta;

                if (nextStateTimer <= 0f)
                {
                    toNextState = false;

                    if (game.stagePositiveResult)
                    {
                        ILivingLetterData runLetterData;

                        runLetterData = game.questionManager.GetlLetterDataSequence()[0];

                        game.runLettersBox.AddRunLetter(runLetterData);
                    }

                    game.Context.GetOverlayWidget().SetStarsScore(game.CurrentStars);
                    game.antura.NextStage();
                    game.SetCurrentState(game.ResultState);
                }
            }

            inputButtonTimer -= delta;

            if (progressInput)
            {
                PlayPositiveAudioFeedback();
                game.eggController.EmoticonPositive();
                game.eggController.StartTrembling();

                progressInput = false;
                if (inputButtonTimer >= 0)
                {
                    inputButtonCount++;
                }
                else
                {
                    inputButtonCount = 0;
                }

                if (inputButtonCount >= inputButtonMax)
                {
                    inputButtonCount = 0;
                    PositiveFeedback();
                }

                inputButtonTimer = inputButtonTime;
            }
        }

        public void UpdatePhysics(float delta) { }

        public void OnEggButtonPressed(ILivingLetterData letterData)
        {
            bool isSequence = game.questionManager.IsSequence();

            if (letterData == game.questionManager.GetlLetterDataSequence()[letterOnSequence])
            {
                if (isSequence)
                {
                    PositiveFeedback();
                    game.eggButtonBox.GetEggButton(letterData).SetOnPressedColor();
                }
                else
                {
                    progressInput = true;
                }
            }
            else
            {
                NegativeFeedback();
            }
        }

        void OnEggPressed()
        {
            DisableAllGameplayInput();

            game.eggController.EmoticonInterrogative();

            game.eggController.PlayAudioQuestion(delegate () { game.eggController.EmoticonClose(); EnableAllGameplayInput(); });
        }

        void PositiveFeedback()
        {
            if (!gameModeOneLetter)
            {
                letterOnSequence++;
            }

            questionProgress++;

            PlayPositiveAudioFeedback();

            float crackingProgress = (float)questionProgress / (float)correctAnswers;

            game.eggController.Cracking(crackingProgress);

            if (crackingProgress == 1f)
            {
                game.eggController.EmoticonHappy();
                DisableAllGameplayInput();
            }
            else
            {
                game.eggController.EmoticonPositive();
            }
        }

        void NegativeFeedback()
        {
            DisableAllGameplayInput();

            bool goAntura = false;

            if (!game.eggController.isNextToExit)
            {
                if (game.antura.IsAnturaIn())
                {
                    goAntura = true;
                }
            }

            game.eggController.EmoticonNegative();

            game.Context.GetAudioManager().PlaySound(Sfx.LetterSad);

            letterOnSequence = 0;

            questionProgress = 0;
            game.eggController.ResetCrack();
            game.eggButtonBox.SetButtonsOnStandardColor();

            if (goAntura)
            {
                AnturaEnter();
                game.eggController.MoveNext(1f, null);
            }
            else
            {
                game.eggController.MoveNext(1f, EnableAllGameplayInput);
            }
        }

        void AnturaExit()
        {
            game.antura.Exit(EnableAllGameplayInput);
        }

        void AnturaEnter()
        {
            game.antura.Enter(AnturaButtonsOut);
        }

        void AnturaButtonsOut()
        {
            game.eggButtonBox.AnturaButtonOut(0.5f, 1f, AnturaSetOnSpitPostion);
        }

        void AnturaButtonsIn()
        {
            game.eggButtonBox.AnturaButtonIn(0.5f, 1f, 1f, 0.15f, game.antura.DoSpit, AnturaExit);
        }

        void AnturaSetOnSpitPostion()
        {
            game.antura.SetOnSpitPosition(AnturaButtonsIn);
        }

        void OnEggExitComplete()
        {
            DisableAllGameplayInput();
            game.stagePositiveResult = false;
            toNextState = true;
            game.eggButtonBox.SetButtonsOnStandardColor();
        }

        void OnEggCrackComplete()
        {
            game.correctStages++;

            DisableAllGameplayInput();
            game.stagePositiveResult = true;

            game.eggButtonBox.SetButtonsOnStandardColor();

            bool isSequence = game.questionManager.IsSequence();

            if (isSequence)
            {
                game.eggButtonBox.PlayButtonsAudio(true, false, 1f, OnLightUpButtonsComplete);
            }
            else
            {
                game.eggButtonBox.GetButtons(false)[0].PlayButtonAudio(true, 1f, OnLightUpButtonsComplete);
            }
        }

        void OnLightUpButtonsComplete()
        {
            bool isSequence = game.questionManager.IsSequence();

            if (isSequence)
            {
                game.eggButtonBox.SetButtonsOnPressedColor();
            }
            else
            {
                game.eggButtonBox.GetEggButton(game.questionManager.GetlLetterDataSequence()[0]).SetOnPressedColor();

            }

            toNextState = true;
        }

        void EnableAllGameplayInput()
        {
            game.eggButtonBox.EnableButtonsInput();
            game.eggController.EnableInput();
        }

        void DisableAllGameplayInput()
        {
            game.eggButtonBox.DisableButtonsInput();
            game.eggController.DisableInput();
        }

        void PlayPositiveAudioFeedback()
        {
            if (positiveAudioSource != null && positiveAudioSource.IsPlaying)
            {
                return;
            }

            positiveAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.LetterHappy);
        }
    }
}