using System;
using UnityEngine;

namespace EA4S.Scanner
{
	public class ScannerResultState : IGameState
	{
		ScannerGame game;

		bool goToEndGame;

		float timer = 4;
		public ScannerResultState(ScannerGame game)
		{
			this.game = game;
		}

		public void EnterState()
		{
			game.isTimesUp = true;
			timer = 4;
			goToEndGame = false;

			game.Context.GetAudioManager().PlayMusic(Music.Relax);

			if(game.isTimesUp)
			{
				game.Context.GetPopupWidget().ShowTimeUp(OnPopupTimeUpCloseRequested);
			}

		}

		public void ExitState()
		{
		}

		void OnPopupTimeUpCloseRequested()
		{
			game.Context.GetPopupWidget().Hide();
			timer = 1;
			goToEndGame = true;
		}

		public void Update(float delta)
		{
			if(!game.isTimesUp || goToEndGame)
				timer -= delta;

			if (timer < 0)
			{
				Debug.Log("Stars: " + game.CurrentStars);
				Debug.Log("Score: " + game.CurrentScore);

				game.EndGame(game.CurrentStars, game.CurrentScoreRecord);
			}
		}

		public void UpdatePhysics(float delta)
		{
		}
	}
}
