namespace EA4S.MakeFriends
{
    public class MakeFriendsIntroductionState : IGameState
    {
        MakeFriendsGame game;

        float timer = 1;
        public MakeFriendsIntroductionState(MakeFriendsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.QuestionState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}