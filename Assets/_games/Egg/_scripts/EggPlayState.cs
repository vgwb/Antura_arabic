using UnityEngine;

namespace EA4S.Egg
{
    public class EggPlayState : IGameState
    {
        EggGame game;

        int letterOnSequence;
        bool isSequence;

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

        bool showTutorial;
        bool tutorialActive;
        int tutorialSequenceIndex;
        float tutorialTimer;
        bool tutorialMarkWrong;
        bool tutorialFirstTime;

        public EggPlayState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            letterOnSequence = 0;

            isSequence = game.questionManager.IsSequence();

            questionProgress = 0;

            if(isSequence)
            {
                correctAnswers = game.questionManager.GetlLetterDataSequence().Count;
            }
            else
            {
                correctAnswers = 3;
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

            game.eggButtonBox.SetOnPressedCallback(OnEggButtonPressed);

            showTutorial = game.showTutorial;
            tutorialActive = false;
            tutorialMarkWrong = false;
            tutorialFirstTime = showTutorial;

            if(tutorialFirstTime)
            {
                TutorialPressCorrect();
            }
        }

        public void ExitState()
        {
            game.eggButtonBox.SetOnPressedCallback(null);
        }

        public void Update(float delta)
        {
            if (toNextState)
            {
                nextStateTimer -= delta;

                if (nextStateTimer <= 0f)
                {
                    toNextState = false;

                    if(!showTutorial)
                    {
                        if (game.stagePositiveResult)
                        {
                            game.correctStages++;

                            ILivingLetterData runLetterData;
                            runLetterData = game.questionManager.GetlLetterDataSequence()[0];
                            game.runLettersBox.AddRunLetter(runLetterData);
                        }
                        
                        game.Context.GetOverlayWidget().SetStarsScore(game.CurrentStars);
                        game.currentStage++;
                        game.antura.NextStage();
                    }

                    game.SetCurrentState(game.ResultState);
                }
            }

            inputButtonTimer -= delta;

            if (progressInput)
            {
                PlayPositiveAudioFeedback();
                game.eggController.EmoticonPositive();
                game.eggController.StartShake();

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

            if(tutorialActive)
            {
                tutorialTimer -= delta;
                if(tutorialTimer <= 0f)
                {
                    if (tutorialMarkWrong)
                    {
                        tutorialMarkWrong = false;
                        TutorialPressCorrect();
                    }
                    else if (isSequence && !tutorialMarkWrong)
                    {
                        tutorialSequenceIndex++;
                        if (tutorialSequenceIndex < correctAnswers)
                        {
                            tutorialTimer = 1f;
                            TutorialUI.Click(game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position);
                        }
                        else
                        {
                            tutorialFirstTime = false;
                            tutorialActive = false;
                            EnableAllGameplayInput();
                        }
                    }
                    else
                    {
                        tutorialFirstTime = false;
                        tutorialActive = false;
                        EnableAllGameplayInput();
                    }
                }
            }
        }

        public void UpdatePhysics(float delta) { }

        public void OnEggButtonPressed(ILivingLetterData letterData)
        {
            if (tutorialFirstTime)
                return;

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
                if(showTutorial)
                {
                    TutorialPressedWrong();
                }
                else
                {
                    NegativeFeedback();
                }
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
            if (isSequence)
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
                game.eggController.ParticleWinEnabled();
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
            game.eggButtonBox.AnturaButtonIn(0.5f, 0.5f, 0.05f, 0.25f, game.antura.DoSpit, AnturaExit);
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
            DisableAllGameplayInput();
            game.stagePositiveResult = true;

            game.eggButtonBox.SetButtonsOnStandardColor();

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

        void TutorialPressCorrect()
        {
            DisableAllGameplayInput();

            tutorialActive = true;
            tutorialSequenceIndex = 0;

            if (isSequence)
            {
                tutorialTimer = 1f;
                TutorialUI.Click(game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position);
            }
            else
            {
                tutorialTimer = 2f;
                TutorialUI.ClickRepeat(game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position, tutorialTimer);
            }
        }

        void TutorialPressedWrong()
        {
            DisableAllGameplayInput();

            letterOnSequence = 0;
            questionProgress = 0;
            game.eggButtonBox.SetButtonsOnStandardColor();

            tutorialActive = true;
            tutorialMarkWrong = true;
        }
    }
}