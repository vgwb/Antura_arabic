namespace EA4S.Scanner
{
	public class ScannerIntroductionState : IGameState
	{
		ScannerGame game;

		float timer = 1;
		public ScannerIntroductionState(ScannerGame game)
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
				game.SetCurrentState(game.PlayState);
				return;
			}
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}
