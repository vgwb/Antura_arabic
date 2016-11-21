namespace EA4S.HideAndSeek
{
    public class IntroductionGameState : IGameState
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
                game.SetCurrentState(game.TutorialState);//questionState
            }
        }

        public void UpdatePhysics(float delta) { }
    }
}