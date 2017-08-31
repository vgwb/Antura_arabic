using Antura.Audio;
using Antura.Minigames;

namespace Antura.Minigames.Scanner
{
	public class ScannerPlayState : IState
	{
		ScannerGame game;

//		float timer = 1;

		public ScannerPlayState(ScannerGame game)
		{
			this.game = game;
		}

		public void EnterState()
		{
			game.ResetScore();

			AudioManager.I.PlayMusic(Music.Theme6);

			game.roundsManager.Initialize();
			game.roundsManager.onRoundsFinished += OnRoundsFinished;


//			StartRound();
		}

		void OnRoundsFinished(int numberOfRoundsWon)
		{
            game.gameActive = false;
			game.CurrentScoreRecord = numberOfRoundsWon;
            game.SetCurrentState(game.ResultState);
			return;
		}

		public void ExitState()
		{
		}

		public void Update(float delta)
		{
//			timer -= delta;
//
//			if (timer < 0)
//			{
//				game.SetCurrentState(game.ResultState);
//				return;
//			}
		}

		public void UpdatePhysics(float delta)
		{
		}


	}
}
