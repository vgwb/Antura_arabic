using EA4S.MinigamesCommon;

namespace EA4S.Balloons
{
    public class BalloonsQuestionState : IState
    {
        BalloonsGame game;
        
        public BalloonsQuestionState(BalloonsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            var popupWidget = game.Context.GetPopupWidget();
            popupWidget.Show();
            popupWidget.SetButtonCallback(OnQuestionCompleted);
            popupWidget.SetMessage("");
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.PlayState);
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        public void Update(float delta)
        {
            game.SetCurrentState(game.PlayState);
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
