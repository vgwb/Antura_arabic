using System;
using TMPro;
using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{
	public class ScannerPlayState : IGameState
	{
		ScannerGame game;

		float timer = 1;

		bool isPlaying = false;
		IAudioSource wordAudioSource;

		private int numberOfRoundsWon = 0;
		private int numberOfRoundsPlayed = 0;
		private int numberOfFailedMoves = 0;

		enum Level { Level1, Level2, Level3, Level4, Level5, Level6 };

		private Level currentLevel = Level.Level4;

		public ScannerPlayState(ScannerGame game)
		{
			this.game = game;
		}

		public void EnterState()
		{
			game.ResetScore();

			game.Context.GetAudioManager().PlayMusic(Music.MainTheme);

			StartRound();
			isPlaying = true;
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

		private void StartRound()
		{

			numberOfRoundsPlayed++;

			game.wordData = AppManager.Instance.Teacher.GimmeAGoodWordData();

			game.letterObjectView.Init(game.wordData);

			Debug.Log("[Scanner] Round: " + numberOfRoundsPlayed);
			numberOfFailedMoves = 0;

			if (game.pedagogicalLevel == 0f) // TODO for testing only each round increment Level. Remove later!
			{
				switch (numberOfRoundsPlayed)
				{
				case 1: 
				case 2: currentLevel = Level.Level1;
					break;
				case 3: currentLevel = Level.Level4;
					break;
				case 4: currentLevel = Level.Level2;
					break;
				case 5: 
				case 6: currentLevel = Level.Level3;
					break;
				default: currentLevel = Level.Level3;
					break;
				}
			}
			else
			{
				// TODO Move later to Start method
				var numberOfLevels = Enum.GetNames(typeof(Level)).Length;
				currentLevel = (Level) Mathf.Clamp((int) Mathf.Floor(game.pedagogicalLevel * numberOfLevels),0, numberOfLevels - 1);
			}
				
			SetLevel(currentLevel);
		}



		IEnumerator EndGame_Coroutine()
		{
			isPlaying = false;

			yield return new WaitForSeconds(1f);

			//			endGameCanvas.gameObject.SetActive(true);

			int numberOfStars = 0;

			if (numberOfRoundsWon <= 0) {
				numberOfStars = 0;
				WidgetSubtitles.I.DisplaySentence("game_result_retry");
			} else if ((float)numberOfRoundsWon / game.numberOfRounds < 0.5f) {
				numberOfStars = 1;
				WidgetSubtitles.I.DisplaySentence("game_result_fair");
			} else if (numberOfRoundsWon < game.numberOfRounds) {
				numberOfStars = 2;
				WidgetSubtitles.I.DisplaySentence("game_result_good");
			} else {
				numberOfStars = 3;
				WidgetSubtitles.I.DisplaySentence("game_result_great");
			}

			LoggerEA4S.Log("minigame", "Scanner", "correctWords", numberOfRoundsWon.ToString());
			LoggerEA4S.Log("minigame", "Scanner", "wrongWords", (game.numberOfRounds - numberOfRoundsWon).ToString());
			LoggerEA4S.Save();

			//			starFlowers.Show(numberOfStars);
		}

		public void CorrectMove(bool roundWon)
		{
			AudioManager.I.PlayDialog("comment_welldone");

			if (roundWon) 
			{			
				//				dancingDotsLL.ShowRainbow();
				game.StartCoroutine(PoofOthers(game.suitcases));
				game.StartCoroutine(RoundWon());
			}
			else
			{
				//				More needed to be done
			}
		}

		public void WrongMove(Vector3 pos)
		{
			numberOfFailedMoves++;

			if (numberOfFailedMoves >= game.allowedFailedMoves)
			{
				game.StartCoroutine(RoundLost());
			}

		}

		IEnumerator CheckNewRound()
		{
			if (numberOfRoundsPlayed >= game.numberOfRounds)
			{
				EndGame();
			}
			else
			{
				yield return new WaitForSeconds(0.25f);
				StartRound();
				yield return new WaitForSeconds(0.75f);
			}
		}

		IEnumerator RoundLost()
		{
			yield return new WaitForSeconds(0.5f);
			AudioManager.I.PlaySfx(Sfx.Lose);

			game.StartCoroutine(PoofOthers(game.suitcases));

			yield return new WaitForSeconds(1.5f);

			game.StartCoroutine(CheckNewRound());
		}

		IEnumerator RoundWon()
		{
			numberOfRoundsWon++;

			yield return new WaitForSeconds(0.25f);

			AudioManager.I.PlaySfx(Sfx.Win);
			yield return new WaitForSeconds(1f);

			game.StartCoroutine(CheckNewRound());
		}

		private void EndGame()
		{
			game.StartCoroutine(EndGame_Coroutine());
		}


		IEnumerator PoofOthers(ScannerSuitcase[] draggables)
		{
			foreach (ScannerSuitcase ss in draggables)
			{
				if (ss.gameObject.activeSelf)
				{
					yield return new WaitForSeconds(0.25f);
					ss.gameObject.SetActive(false);
					game.CreatePoof(ss.transform.position, 2f, true);
				}

			}
		}


		public Color32 SetAlpha(Color32 color, byte alpha)
		{
			if (alpha >= 0 && alpha <= 255)
			{
				return new Color32(color.r, color.g, color.b, alpha);
			}
			else
			{
				return color;
			}
		}




		private void SetLevel(Level level)
		{
			//			foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.gameObject.SetActive(false);
			//			foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.gameObject.SetActive(false);
			//			foreach (GameObject go in diacritics) go.SetActive(false);

			switch (level)
			{
			case Level.Level1 :
				break;

			case Level.Level2 : 
				break;

			case Level.Level3 :
				break;

			case Level.Level4 : 
				break;

			case Level.Level5 : 
				break;

			case Level.Level6 :
				break;

			default:
				SetLevel(Level.Level1);
				break;

			}
		}

	}
}
