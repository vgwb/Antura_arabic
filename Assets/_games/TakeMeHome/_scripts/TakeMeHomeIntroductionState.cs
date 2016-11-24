namespace EA4S.TakeMeHome
{
	public class TakeMeHomeIntroductionState : IGameState
	{
		TakeMeHomeGame game;

		int currentShownTubes = 2;



		public TakeMeHomeIntroductionState(TakeMeHomeGame game)
		{
			this.game = game;


		}

		public void EnterState()
		{
            
			game.IncrementRound();

			if (game.currentRound > 6) {
				game.SetCurrentState(game.EndState);
			}
		}

		public void ExitState()
		{
		}

		public void Update(float delta)
		{
			if (game.currentLetter != null && !game.currentLetter.isMoving) {
                game.currentLetter.sayLetter();

                game.currentLetter.isDraggable = true;
				game.SetCurrentState(game.PlayState);
			}
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}