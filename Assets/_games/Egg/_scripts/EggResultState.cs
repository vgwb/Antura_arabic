namespace EA4S.Egg
{
    public class EggResultState : IGameState
    {
        EggGame game;

        public EggResultState(EggGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.currentStage++;

            if(game.currentStage < EggGame.numberOfStage)
            {
                if (game.stagePositiveResult)
                {
                    game.Context.GetPopupWidget().Show(OnPopupCloseRequested, TextID.GAME_RESULT_GOOD, true);
                }
                else
                {
                    game.Context.GetPopupWidget().Show(OnPopupCloseRequested, TextID.GAME_RESULT_RETRY, false);
                }
            }
            else
            {
                game.eggButtonBox.RemoveButtons();
                game.EndGame(game.CurrentStars, game.correctStages);
            }
        }

        public void ExitState()
        {
            
        }

        public void Update(float delta)
        {
            
        }

        public void UpdatePhysics(float delta)
        {
            
        }

        void OnPopupCloseRequested()
        {
            game.Context.GetPopupWidget().Hide();
            game.SetCurrentState(game.QuestionState);
        }
    }
}