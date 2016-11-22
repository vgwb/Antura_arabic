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
        bool tutorialCorrectActive;
        int tutorialSequenceIndex;
        float tutorialCorrectTimer;

        float tutorialDelayTimer;
        float tutorialDelayTime = 3f;
        bool tutorialStop;

        bool isPlayDialogue;

        public EggPlayState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            letterOnSequence = 0;

            isSequence = game.questionManager.IsSequence();

            questionProgress = 0;

            if (isSequence)
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
            tutorialCorrectActive = false;
            tutorialDelayTimer = tutorialDelayTime;
            tutorialStop = false;

            isPlayDialogue = false;

            if (showTutorial)
            {
                TutorialPressCorrect();

                if(isSequence)
                {
                    isPlayDialogue = true;
                    game.Context.GetAudioManager().PlayDialogue(TextID.EGG_TUTO_SEQUENCE, delegate() { isPlayDialogue = false; });
                }
                else
                {
                    isPlayDialogue = true;
                    game.Context.GetAudioManager().PlayDialogue(TextID.EGG_TUTO_BUTTON, delegate () { isPlayDialogue = false; });
                }
            }

            if (!showTutorial)
            {
                game.InitializeOverlayWidget();
            }

            EnableAllGameplayInput();
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

                    if (!showTutorial)
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
                game.eggController.ParticleCorrectEnabled();

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

            if(showTutorial && !tutorialStop)
            {
                if (tutorialCorrectActive)
                {
                    tutorialCorrectTimer -= delta;
                    if (tutorialCorrectTimer <= 0f)
                    {
                        if (isSequence)
                        {
                            tutorialSequenceIndex++;
                            if (tutorialSequenceIndex < correctAnswers)
                            {
                                tutorialCorrectTimer = 1f;

                                Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position;
                                TutorialUI.Click(clickPosition);
                            }
                            else
                            {
                                tutorialCorrectActive = false;
                                tutorialDelayTimer = tutorialDelayTime;
                            }
                        }
                        else
                        {
                            tutorialCorrectActive = false;
                            tutorialDelayTimer = tutorialDelayTime;
                        }
                    }
                }
                else
                {
                    tutorialDelayTimer -= delta;

                    if(tutorialDelayTimer <= 0f)
                    {
                        TutorialPressCorrect();
                    }
                }
            }
        }

        public void UpdatePhysics(float delta) { }

        public void OnEggButtonPressed(ILivingLetterData letterData)
        {
            //if (isPlayDialogue)
            //    return;

            game.Context.GetAudioManager().PlaySound(Sfx.UIButtonClick);

            if (showTutorial)
            {
                TutorialUI.Clear(false);
                tutorialDelayTimer = tutorialDelayTime;
                tutorialCorrectActive = false;
            }

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
                game.eggButtonBox.SetButtonsOnStandardColor(game.eggButtonBox.GetEggButton(letterData));

                if (showTutorial)
                {
                    TutorialPressedWrong(letterData);
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

            float crackingProgress = (float)questionProgress / (float)correctAnswers;

            game.eggController.Cracking(crackingProgress);

            game.eggController.ParticleCorrectEnabled();

            if (crackingProgress == 1f)
            {
                game.Context.GetAudioManager().PlaySound(Sfx.EggBreak);
                game.eggController.EmoticonHappy();
                game.eggController.ParticleWinEnabled();
                DisableAllGameplayInput();
                tutorialStop = true;
            }
            else
            {
                PlayPositiveAudioFeedback();
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

                game.Context.GetAudioManager().PlaySound(Sfx.ScaleUp);
                tutorialStop = true;
            }
            else
            {
                game.Context.GetAudioManager().PlaySound(Sfx.ScaleDown);
            }

            game.eggController.EmoticonNegative();

            //game.Context.GetAudioManager().PlaySound(Sfx.LetterSad);

            letterOnSequence = 0;
            questionProgress = 0;
            game.eggController.ResetCrack();

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
            tutorialStop = true;
            DisableAllGameplayInput();
            game.stagePositiveResult = false;
            toNextState = true;
            game.eggButtonBox.SetButtonsOnStandardColor();
        }

        void OnEggCrackComplete()
        {
            tutorialStop = true;
            DisableAllGameplayInput();
            game.stagePositiveResult = true;
            
            game.eggButtonBox.SetButtonsOnStandardColor();
            game.eggController.ParticleWinDisabled();

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

            //positiveAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.LetterHappy);
            positiveAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.EggMove);
        }

        void TutorialPressCorrect()
        {
            tutorialCorrectActive = true;
            //tutorialSequenceIndex = 0;
            tutorialSequenceIndex = letterOnSequence;

            if (isSequence)
            {
                tutorialCorrectTimer = 1f;

                Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position;
                TutorialUI.Click(clickPosition);
            }
            else
            {
                tutorialCorrectTimer = 2f;

                Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position;
                TutorialUI.ClickRepeat(clickPosition, tutorialCorrectTimer);
            }
        }

        void TutorialPressedWrong(ILivingLetterData letterData)
        {
            letterOnSequence = 0;
            questionProgress = 0;

            Vector3 markPosition = game.eggButtonBox.GetEggButton(letterData).transform.position;

            TutorialUI.MarkNo(markPosition, TutorialUI.MarkSize.Normal);
        }
    }
}