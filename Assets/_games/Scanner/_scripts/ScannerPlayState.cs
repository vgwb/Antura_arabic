using System;
using TMPro;
using UnityEngine;

namespace EA4S.Scanner
{
	public class ScannerPlayState : IGameState
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
			ScannerConfiguration.Instance.gameActive = false;
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
