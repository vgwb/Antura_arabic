namespace EA4S.Egg
{
    public class EggPlayState : IGameState
    {
        EggGame game;

        int letterOnSequence;
        bool gameModeOneLetter;

        int questionProgress;
        int correctAnswers;

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
        }

        public void ExitState()
        {
            DisableAllGameplayInput();
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {

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
            game.SetCurrentState(game.ResultState);
        }

        void OnEggCrackComplete()
        {
            game.SetCurrentState(game.ResultState);
        }
    }
}