namespace EA4S.MakeFriends
{
    public class MakeFriendsPlayState : IGameState
    {
        MakeFriendsGame game;

        public MakeFriendsPlayState(MakeFriendsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            MinigamesUI.Init(MinigamesUIElement.Starbar);
            game.PlayActiveMusic();
            game.Play();
        }

        public void ExitState()
        {
        }

        public void OnResult()
        {
            game.SetCurrentState(game.ResultState);
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
