using Antura.MinigamesCommon;

namespace Antura.Minigames.HideAndSeek
{
    public class IntroductionGameState : IState
    {
		HideAndSeekGame game;

        float timer = 1;
		public IntroductionGameState(HideAndSeekGame game)
        {
            this.game = game;
        }

        public void EnterState() {}

        public void ExitState() { }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                if (game.TutorialEnabled)
                {
                    game.SetCurrentState(game.TutorialState);
                } else {
                    game.SetCurrentState(game.PlayState);
                }
            }
        }

        public void UpdatePhysics(float delta) { }
    }
}