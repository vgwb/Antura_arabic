namespace EA4S.Tobogan
{
    public class ToboganPlayState : IGameState
    {
        CountdownTimer gameTime = new CountdownTimer(99.9f);
        ToboganGame game;
        
        public ToboganPlayState(ToboganGame game)
        {
            this.game = game;

            gameTime.onTimesUp += OnTimesUp;
        }

        public void EnterState()
        {
            game.CurrentAnswersRecord = 0;
            
            // Reset game timer
            gameTime.Reset();
            gameTime.Start();

            game.questionsManager.Initialize();
            game.questionsManager.NewQuestion();

            // Show questions description
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show(OnPopupCloseRequested, ToboganConfiguration.Instance.PipeQuestions.GetDescription(), true);
        }

        void OnPopupCloseRequested()
        {
            game.Context.GetPopupWidget().Hide();
        }

        public void ExitState()
        {

            game.Context.GetPopupWidget().Hide();
            gameTime.Stop();
        }

        public void Update(float delta)
        {
            if (game.CurrentAnswersRecord >= ToboganGame.MAX_ANSWERS_RECORD)
            {
                // Maximum tower height reached
                game.SetCurrentState(game.ResultState);
                return;
            }

            gameTime.Update(delta);
        }

        public void UpdatePhysics(float delta)
        {

        }

        void OnTimesUp()
        {
            // Time's up!
            game.SetCurrentState(game.ResultState);
        }
    }
}