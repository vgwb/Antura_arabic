namespace EA4S.TakeMeHome
{
	public class TakeMeHomeResetState : IGameState {

		TakeMeHomeGame game;
		bool win;

		public TakeMeHomeResetState(TakeMeHomeGame game)
		{
			this.game = game;
			win = false;
		}

		public void EnterState()
		{
           

            if (game.currentLetter.lastTube == null) {
				win = false;
				game.currentLetter.respawn = true;
				return;
			}

			int tubeIndex = int.Parse(game.currentLetter.lastTube.name.Substring (5));

			win = false;
			if (tubeIndex == game.currentTube) {
				AudioManager.I.PlaySfx (Sfx.Win);
				win = true;
				game.IncrementScore ();

			} else {
				AudioManager.I.PlaySfx (Sfx.Lose);

			}

			game.currentLetter.followTube (win);
		}

		public void ExitState()
		{
			game.currentLetter.isDraggable = true;
		}

		public void Update(float delta)
		{
			if (!game.currentLetter.isMoving) {
				if(!win)
					game.SetCurrentState (game.PlayState);
				else
					game.SetCurrentState (game.IntroductionState);
			}
		}

		public void UpdatePhysics(float delta)
		{
		}


	}
}
