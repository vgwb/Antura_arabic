namespace EA4S.MakeFriends
{
    public class MakeFriendsPlayGameState : IGameState
    {
        MakeFriendsGame game;

        float timer = 4;
        public MakeFriendsPlayGameState(MakeFriendsGame game)
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
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
