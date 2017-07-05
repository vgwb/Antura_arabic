using Antura.UI;

namespace Antura.Minigames.Balloons
{
    public class BalloonsPlayState : IState
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
