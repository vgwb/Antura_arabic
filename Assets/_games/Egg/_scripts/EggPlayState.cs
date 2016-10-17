namespace EA4S.Egg
{
    public class EggPlayState : IGameState
    {
        EggGame game;

        int letterOnSequence;
        bool gameModeOneLetter;

        int questionProgress;
        int correctAnswers;

        bool anturaEnter;
        bool anturaEntered;

        float nextStateTimer;
        bool toNextState;

        public EggPlayState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            letterOnSequence = 0;

            gameModeOneLetter = game.questionManager.GetQuestionWordData() == null;

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

            anturaEnter = false;
            anturaEntered = false;

            nextStateTimer = 2f;
            toNextState = false;
        }

        public void ExitState()
        {
            
        }

        public void Update(float delta)
        {
            if(anturaEnter && !anturaEntered)
            {
                anturaEntered = true;
                AnturaOut();
            }

            if(toNextState)
            {
                nextStateTimer -= delta;
                
                if(nextStateTimer <= 0f)
                {
                    toNextState = false;
                    game.SetCurrentState(game.ResultState);
                }
            }
        }

        public void UpdatePhysics(float delta)
        {

        }

        void AnturaOut()
        {
            game.antura.Bark();
            DisableAllGameplayInput();
            game.eggButtonBox.AnturaButtonOut(AnturaIn, 0.5f, 1f);
        }

        void AnturaIn()
        {
            game.eggButtonBox.AnturaButtonIn(EnableAllGameplayInput, 0.5f, 1f);
        }

        public void OnEggButtonPressed(ILivingLetterData letterData)
        {
            if (letterData == game.questionManager.GetlLetterDataSequence()[letterOnSequence])
            {
                PositiveFeedback();
            }
            else
            {
                NegativeFeedback();
            }
        }

        void PositiveFeedback()
        {
            if (!gameModeOneLetter)
            {
                letterOnSequence++;
            }

            questionProgress++;
            game.eggController.Cracking(questionProgress / correctAnswers);
        }

        void NegativeFeedback()
        {
            anturaEnter = true;

            letterOnSequence = 0;

            questionProgress = 0;
            game.eggController.ResetCrack();

            DisableAllGameplayInput();
            game.eggController.MoveNext(1f, EnableAllGameplayInput);
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

            WordData questionWordData = game.questionManager.GetQuestionWordData();

            if (questionWordData == null)
            {
                OnLightUpButtonsComplete();
            }
            else
            {
                game.eggButtonBox.LightUpButtons(true, false, 1f, 1f, OnLightUpButtonsComplete);
            }

        }

        void OnLightUpButtonsComplete()
        {
            WordData questionWordData = game.questionManager.GetQuestionWordData();

            if (questionWordData == null)
            {
                game.eggButtonBox.GetButtons(false)[0].LightUp(true, 1f, 1, null);
            }
            else
            {
                game.Context.GetAudioManager().PlayWord(questionWordData.Key);
            }

            toNextState = true;
        }
    }
}