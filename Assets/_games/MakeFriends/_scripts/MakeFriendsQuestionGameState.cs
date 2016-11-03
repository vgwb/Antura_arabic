namespace EA4S.MakeFriends
{
    public class MakeFriendsQuestionGameState : IGameState
    {
        MakeFriendsGame game;
        
        public MakeFriendsQuestionGameState(MakeFriendsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetPopupWidget().Show(OnQuestionCompleted, TextID.ASSESSMENT_RESULT_GOOD, true, null);
        }

        public void ExitState()
        {
            game.Context.GetPopupWidget().Hide();
        }

        void OnQuestionCompleted()
        {
            game.SetCurrentState(game.PlayState);
        }

        public void Update(float delta)
        {

        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
