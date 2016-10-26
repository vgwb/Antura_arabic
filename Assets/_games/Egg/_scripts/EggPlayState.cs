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

        float anturaProbabilityOfIn;

        float inputButtonTime = 0.3f;
        float inputButtonTimer;
        int inputButtonCount;
        int inputButtonMax = 4;
        bool progressInput;

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

            EnableAllGameplayInput();

            if (game.gameDifficulty < 0.25f)
            {
                anturaProbabilityOfIn = 0f;
            }
            else if (game.gameDifficulty < 0.5f)
            {
                anturaProbabilityOfIn = 0.20f;
            }
            else if (game.gameDifficulty < 0.75f)
            {
                anturaProbabilityOfIn = 0.40f;
            }
            else
            {
                anturaProbabilityOfIn = 0.60f;
            }

            nextStateTimer = 2f;
            toNextState = false;

            inputButtonTimer = 0f;
            inputButtonCount = 0;
            progressInput = false;
        }

        public void ExitState()
        {

        }

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

                    game.SetCurrentState(game.ResultState);
                }
            }

            inputButtonTimer -= delta;

            if (progressInput)
            {
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

        public void UpdatePhysics(float delta)
        {

        }

        public void OnEggButtonPressed(ILivingLetterData letterData)
        {
            bool isSequence = game.questionManager.IsSequence();

            if (letterData == game.questionManager.GetlLetterDataSequence()[letterOnSequence])
            {
                if (isSequence)
                {
                    PositiveFeedback();
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

        public void OnEggPressed()
        {
            DisableAllGameplayInput();

            bool isSequence = game.questionManager.IsSequence();

            if (isSequence)
            {
                game.eggButtonBox.LightUpButtons(true, true, false, 1f, 1f, EnableAllGameplayInput);
            }
            else
            {
                game.Context.GetAudioManager().PlayLetter(((LL_LetterData)game.questionManager.GetlLetterDataSequence()[0]));

                EnableAllGameplayInput();
            }
        }

        void PositiveFeedback()
        {
            if (!gameModeOneLetter)
            {
                letterOnSequence++;
            }

            questionProgress++;

            if ((questionProgress / correctAnswers) == 1f)
            {
                game.Context.GetAudioManager().PlaySound(Sfx.Hit);
            }
            else
            {
                game.Context.GetAudioManager().PlaySound(Sfx.LetterHappy);
            }

            game.eggController.Cracking( (float)questionProgress / (float)correctAnswers );
        }

        void NegativeFeedback()
        {
            DisableAllGameplayInput();

            bool goAntura = false;

            if (!game.eggController.isNextToExit)
            {
                float anturaStartEnter = Random.Range(0f, 1f);

                if (anturaStartEnter < anturaProbabilityOfIn)
                {
                    goAntura = true;
                }
            }

            game.Context.GetAudioManager().PlaySound(Sfx.LetterSad);

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
            game.eggButtonBox.AnturaButtonOut(AnturaButtonsIn, 0.5f, 1f);
        }

        void AnturaButtonsIn()
        {
            game.eggButtonBox.AnturaButtonIn(AnturaExit, 0.5f, 1f);
        }

        void OnEggExitComplete()
        {
            DisableAllGameplayInput();
            game.stagePositiveResult = false;
            toNextState = true;
        }

        void OnEggCrackComplete()
        {
            game.correctStages++;

            DisableAllGameplayInput();
            game.stagePositiveResult = true;

            bool isSequence = game.questionManager.IsSequence();

            if (isSequence)
            {
                game.eggButtonBox.LightUpButtons(false, true, false, 1f, 1f, OnLightUpButtonsComplete);
            }
            else
            {
                OnLightUpButtonsComplete();
            }
        }

        void OnLightUpButtonsComplete()
        {
            bool isSequence = game.questionManager.IsSequence();

            if (!isSequence)
            {
                game.eggButtonBox.GetButtons(false)[0].LightUp(false, true, 1f, 1, null);
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
    }
}