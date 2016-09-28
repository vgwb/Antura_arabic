namespace EA4S.Tobogan
{
    public class ToboganIntroductionState : IGameState
    {
        ToboganGame game;

        float timer = 4;
        public ToboganIntroductionState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            var subTitleWidget = game.Context.GetSubtitleWidget();
            subTitleWidget.DisplaySentence(TextID.GAME_RESULT_RETRY);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.PlayState);
                return;
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
