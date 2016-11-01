namespace EA4S.TakeMeHome
{
	public class TakeMeHomePlayState : IGameState {

		TakeMeHomeGame game;
		CountdownTimer gameTime;

		float timer = 1;
		public TakeMeHomePlayState(TakeMeHomeGame game)
		{
			this.game = game;
			gameTime = new CountdownTimer(UnityEngine.Mathf.Lerp(90.0f, 60.0f, TakeMeHomeConfiguration.Instance.Difficulty));
			gameTime.onTimesUp += OnTimesUp;

			gameTime.Reset();

		}

		public void EnterState()
		{
			gameTime.Start();

			game.timerText.gameObject.SetActive(true);
			game.roundText.gameObject.SetActive(true);




			game.IncrementRound ();
		}

		public void ExitState()
		{
			game.timerText.gameObject.SetActive(false);
			game.roundText.gameObject.SetActive(false);
			gameTime.Stop();
		}

		public void Update(float delta)
		{
			gameTime.Update(delta);
			game.timerText.text = ((int)gameTime.Time).ToString();

			if (game.currentRound > 6) {
				game.SetCurrentState(game.EndState);
			}
		}

		public void UpdatePhysics(float delta)
		{
		}

		void OnTimesUp()
		{
			// Time's up!
			game.isTimesUp = true;
			game.SetCurrentState(game.EndState);
		}
	}
}
