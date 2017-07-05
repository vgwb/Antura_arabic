namespace Antura.Minigames.MakeFriends
{
    public class MakeFriendsResultState : IState
    {
        MakeFriendsGame game;

        float timer = 1.5f;
        public MakeFriendsResultState(MakeFriendsGame game)
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
                game.endGameCanvas.gameObject.SetActive(true);
                game.EndGame(game.CurrentStars, game.CurrentScore);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
