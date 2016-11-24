using System;
using UnityEngine;

namespace EA4S.Scanner
{
	public class ScannerResultState : IGameState
	{
		ScannerGame game;

		float timer = 4;

		public ScannerResultState(ScannerGame game)
		{
			this.game = game;
		}

		public void EnterState()
		{
			timer = 4;
			AudioManager.I.PlayMusic(Music.Relax);

		}

		public void ExitState()
		{
		}

		public void Update(float delta)
		{
			timer -= delta;

			if (timer < 0)
			{
				game.EndGame(game.CurrentStars, game.CurrentScoreRecord);
			}
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}
