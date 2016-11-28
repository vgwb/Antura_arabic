namespace EA4S.Balloons
{
    public class BalloonsPlayState : IGameState
    {
        BalloonsGame game;

        public BalloonsPlayState(BalloonsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer);
            game.PlayIntroVoiceOver();
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
