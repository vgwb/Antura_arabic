namespace EA4S.Tobogan
{
    public class ToboganPlayState : IGameState
    {
        ToboganGame game;
        
        public ToboganPlayState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            WidgetPopupWindow.I.ShowStringAndWord(null, "This is a Text", AppManager.Instance.Teacher.GimmeAGoodWordData());
        }

        public void ExitState()
        {
            WidgetPopupWindow.I.Close();
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}