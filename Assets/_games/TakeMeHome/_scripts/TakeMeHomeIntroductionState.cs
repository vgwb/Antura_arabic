namespace EA4S.TakeMeHome
{
	public class TakeMeHomeIntroductionState : IGameState
	{
		TakeMeHomeGame game;

		float timer = 1;
		public TakeMeHomeIntroductionState(TakeMeHomeGame game)
		{
			this.game = game;


		}

		public void EnterState()
		{
			game.InitTubes ();

			//game.

			game.Context.GetAudioManager().PlayMusic(Music.Theme3);
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
			}
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}