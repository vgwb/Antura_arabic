namespace EA4S.MakeFriends
{
    public class MakeFriendsResultGameState : IGameState
    {
        MakeFriendsGame game;

        float timer = 4;
        public MakeFriendsResultGameState(MakeFriendsGame game)
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
                game.EndGame(game.CurrentStars, game.CurrentScore);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
