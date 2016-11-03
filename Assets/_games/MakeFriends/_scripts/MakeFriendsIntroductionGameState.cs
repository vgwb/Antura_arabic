namespace EA4S.MakeFriends
{
    public class MakeFriendsIntroductionGameState : IGameState
    {
        MakeFriendsGame game;

        float timer = 4;
        public MakeFriendsIntroductionGameState(MakeFriendsGame game)
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