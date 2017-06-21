using EA4S.MinigamesAPI;
using EA4S.MinigamesCommon;
using EA4S.Tutorial;
using UnityEngine;

namespace EA4S.Minigames.Egg
{
    public class EggPlayState : IState
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
        bool repeatInputHasProgressed;
        bool enteredRepeatMode;

        IAudioSource positiveAudioSource;

        bool showTutorial;
        bool tutorialCorrectActive;
        int tutorialSequenceIndex;
        float tutorialCorrectTimer;

        float tutorialDelayTimer;
        float tutorialDelayTime = 3f;
        bool tutorialStop;

        public EggPlayState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            letterOnSequence = 0;

            isSequence = game.CurrentQuestion.IsSequence();

            questionProgress = 0;

            if (isSequence) {
                correctAnswers = game.CurrentQuestion.Letters.Count;
            } else {
                correctAnswers = 3;
            }

            game.eggController.onEggPressedCallback = OnEggPressed;

            EnableAllGameplayInput();

            nextStateTimer = 0f;
            toNextState = false;

            inputButtonTimer = 0f;
            inputButtonCount = 0;
            repeatInputHasProgressed = false;
            enteredRepeatMode = false;

            game.eggButtonBox.SetOnPressedCallback(OnEggButtonPressed);

            showTutorial = game.ShowTutorial;
            tutorialCorrectActive = false;
            tutorialDelayTimer = tutorialDelayTime;
            tutorialStop = false;

            if (showTutorial) {
                ShowTutorialPressCorrect();

                if (isSequence) {
                    game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.Egg_sequence_Tuto);
                } else {
                    game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.Egg_letters_Tuto);
                }
            }

            if (!showTutorial) {
                game.InitializeOverlayWidget();
            }

            EnableAllGameplayInput();
        }

        public void ExitState()
        {
            if (showTutorial) {
                TutorialUI.Clear(true);
            }

            game.eggButtonBox.SetOnPressedCallback(null);
        }

        public void Update(float delta)
        {
            if (toNextState) {
                nextStateTimer -= delta;

                if (nextStateTimer <= 0f) {
                    toNextState = false;

                    if (!showTutorial) {
                        if (game.stagePositiveResult) {
                            game.correctStages++;

                            ILivingLetterData runLetterData;
                            runLetterData = game.CurrentQuestion.Letters[0];
                            game.runLettersBox.AddRunLetter(runLetterData);
                        }

                        game.Context.GetOverlayWidget().SetStarsScore(game.correctStages);
                        game.currentStage++;
                        game.antura.NextStage();
                    }

                    game.SetCurrentState(game.ResultState);
                }
            }

            inputButtonTimer -= delta;

            if (repeatInputHasProgressed) {
                PlayPositiveAudioFeedback();

                game.eggController.EmoticonPositive();
                game.eggController.StartShake();
                game.eggController.ParticleCorrectEnabled();

                repeatInputHasProgressed = false;
                if (inputButtonTimer >= 0) {
                    inputButtonCount++;
                } else {
                    inputButtonCount = 0;
                }

                if (inputButtonCount >= inputButtonMax) {
                    inputButtonCount = 0;
                    PositiveFeedback();
                }

                tutorialDelayTimer = 0.5f;

                inputButtonTimer = inputButtonTime;
            }

            if (showTutorial && !tutorialStop) {
                if (tutorialCorrectActive) {
                    tutorialCorrectTimer -= delta;
                    if (tutorialCorrectTimer <= 0f) {
                        if (isSequence) {
                            tutorialSequenceIndex++;
                            if (tutorialSequenceIndex < correctAnswers) {
                                tutorialCorrectTimer = 1f;

                                Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position;
                                TutorialUI.Click(clickPosition);
                            } else {
                                tutorialCorrectActive = false;
                                tutorialDelayTimer = tutorialDelayTime;
                            }
                        } else {
                            tutorialCorrectActive = false;
                            tutorialDelayTimer = tutorialDelayTime;
                        }
                    }
                } else {
                    tutorialDelayTimer -= delta;

                    if (tutorialDelayTimer <= 0f) {
                        ShowTutorialPressCorrect();
                    }
                }
            }
        }

        public void UpdatePhysics(float delta) { }

        public void OnEggButtonPressed(ILivingLetterData letterData)
        {
            game.Context.GetAudioManager().PlaySound(Sfx.UIButtonClick);

            if (showTutorial) {
                if (!enteredRepeatMode)
                    TutorialUI.Clear(false);
                tutorialDelayTimer = tutorialDelayTime;
                tutorialCorrectActive = false;
            }

            if (letterData == game.CurrentQuestion.Letters[letterOnSequence]) {
                if (isSequence) {
                    game.eggButtonBox.GetEggButton(letterData).SetPressed();
                    PositiveFeedback();
                    game.Context.GetLogManager().OnAnswered(letterData, true);
                } else {
                    repeatInputHasProgressed = true;

                    if (!enteredRepeatMode) {
                        EggConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.OK);
                        enteredRepeatMode = true;
                        Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[0].transform.position;
                        TutorialUI.ClickRepeat(clickPosition, 4);
                        game.eggButtonBox.RemoveButtons((a) => { return a != letterData; });

                        if (!showTutorial)
                            game.Context.GetLogManager().OnAnswered(letterData, true);
                    }
                }
            } else {
                if (isSequence && game.eggButtonBox.GetEggButton(letterData).IsPressed())
                    return;

                game.eggButtonBox.SetButtonsOnStandardColor(game.eggButtonBox.GetEggButton(letterData));

                if (showTutorial) {
                    ShowTutorialPressedWrong(letterData);
                } else {
                    NegativeFeedback();
                    game.Context.GetLogManager().OnAnswered(letterData, false);
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
            if (isSequence) {
                letterOnSequence++;
            }

            questionProgress++;

            float crackingProgress = (float)questionProgress / (float)correctAnswers;

            game.eggController.Cracking(crackingProgress);

            game.eggController.ParticleCorrectEnabled();

            if (crackingProgress == 1f) {
                game.Context.GetAudioManager().PlaySound(Sfx.EggBreak);
                game.eggController.EmoticonHappy();
                game.eggController.ParticleWinEnabled();
                DisableAllGameplayInput();
                tutorialStop = true;
                TutorialUI.Clear(false);

                OnEggCrackComplete();
            } else {
                PlayPositiveAudioFeedback();
                game.eggController.EmoticonPositive();
            }
        }

        void NegativeFeedback()
        {
            DisableAllGameplayInput();

            bool goAntura = false;

            if (!game.eggController.isNextToExit) {
                if (game.antura.IsAnturaIn()) {
                    goAntura = true;
                }

                game.Context.GetAudioManager().PlaySound(Sfx.ScaleUp);
                tutorialStop = true;
            } else {
                game.Context.GetAudioManager().PlaySound(Sfx.ScaleDown);
                OnEggExitComplete();
            }

            game.eggController.EmoticonNegative();
            EggConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.KO);

            letterOnSequence = 0;
            questionProgress = 0;
            game.eggController.ResetCrack();

            if (goAntura) {
                AnturaEnter();
                game.eggController.MoveNext(1f, null);
            } else {
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
            //game.eggButtonBox.SetButtonsOnStandardColor();
            game.eggButtonBox.DisableButtonsInput();
        }

        void OnEggCrackComplete()
        {
            tutorialStop = true;
            DisableAllGameplayInput();
            game.stagePositiveResult = true;

            if (isSequence) {
                game.eggButtonBox.PlayButtonsAudio(true, false, 0.5f, OnLightUpButtonsComplete, () => { game.eggButtonBox.SetButtonsOnStandardColor(null, false); });
            } else {
                game.eggButtonBox.GetButtons(false)[0].PlayButtonAudio(true, 0.5f, OnLightUpButtonsComplete, () => { game.eggButtonBox.SetButtonsOnStandardColor(null, false); });
            }
        }

        void OnLightUpButtonsComplete()
        {
            if (isSequence) {
                game.eggButtonBox.SetButtonsOnPressedColor();
            } else {
                //game.eggButtonBox.GetEggButton(game.CurrentQuestion.Letters[0]).SetPressed();
            }

            game.eggController.ParticleWinDisabled();
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
            if (positiveAudioSource != null && positiveAudioSource.IsPlaying) {
                return;
            }

            positiveAudioSource = game.Context.GetAudioManager().PlaySound(Sfx.EggMove);
        }

        void ShowTutorialPressCorrect()
        {
            tutorialCorrectActive = true;
            tutorialSequenceIndex = letterOnSequence;

            if (isSequence) {
                tutorialCorrectTimer = 1f;

                Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position;
                TutorialUI.Click(clickPosition);
            }
            /*
            else
            {
                tutorialCorrectTimer = 2f;

                Vector3 clickPosition = game.eggButtonBox.GetButtons(false)[tutorialSequenceIndex].transform.position;
                TutorialUI.ClickRepeat(clickPosition, tutorialCorrectTimer);
            }
            */
        }

        void ShowTutorialPressedWrong(ILivingLetterData letterData)
        {
            letterOnSequence = 0;
            questionProgress = 0;

            Vector3 markPosition = game.eggButtonBox.GetEggButton(letterData).transform.position;

            TutorialUI.MarkNo(markPosition, TutorialUI.MarkSize.Normal);
        }
    }
}