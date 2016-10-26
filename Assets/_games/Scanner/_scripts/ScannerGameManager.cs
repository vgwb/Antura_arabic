using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using TMPro;

using System;
using EA4S;

namespace EA4S.Scanner
{

	public class ScannerGameManager : MiniGameBase {

		public static ScannerGameManager instance;

		public Canvas endGameCanvas;

		public StarFlowers starFlowers;

		public GameObject antura;
		public float anturaMinDelay = 3f;
		public float anturaMaxDelay = 10f;
		public float anturaMinScreenTime = 1f;
		public float anturaMaxScreenTime = 2f;

		public GameObject poofPrefab;


		public string currentLetter = "";

		[Range(0,1)] public float pedagogicalLevel = 0;

		public int numberOfRounds = 6;

		public int allowedFailedMoves = 3;

		public LetterObjectView letterObjectView;

		public ScannerLivingLetter scannerLL;

		public ScannerSuitcase[] suitcases;

		private int numberOfRoundsWon = 0;
		private int numberOfRoundsPlayed = 0;
		private int numberOfFailedMoves = 0;

		enum Level { Level1, Level2, Level3, Level4, Level5, Level6 };

		private Level currentLevel = Level.Level4;
		private bool isPlaying = false;


		protected override void Awake()
		{
			base.Awake();
			instance = this;
		}

		protected override void Start()
		{

			base.Start();


			AppManager.Instance.InitDataAI();
			AppManager.Instance.CurrentGameManagerGO = gameObject;
			SceneTransitioner.Close();


			StartRound();

			isPlaying = true;

			StartCoroutine(AnimateAntura());

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

		IEnumerator AnimateAntura()
		{
			yield return new WaitForSeconds(1f);
//			Vector3 pos = antura.transform.position;
			// Move antura off screen because SetActive is reseting the animation to running
//			antura.transform.position = new Vector3 (-50,pos.y,pos.z);
			do
			{
				yield return new WaitForSeconds(UnityEngine.Random.Range(anturaMinDelay, anturaMaxDelay));
//				CreatePoof(pos, 2f, false);
				yield return new WaitForSeconds(0.4f);
//				antura.transform.position = pos;
//
				yield return new WaitForSeconds(UnityEngine.Random.Range(anturaMinScreenTime, anturaMaxScreenTime));
//				CreatePoof(pos, 2f, false);
//				antura.transform.position = new Vector3 (-50,pos.y,pos.z);
			} while (isPlaying);

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

		private void StartRound()
		{

			numberOfRoundsPlayed++;

			LL_WordData wordData = AppManager.Instance.Teacher.GimmeAGoodWordData();

			letterObjectView.Init(wordData);

			Debug.Log("[Scanner] Round: " + numberOfRoundsPlayed);
			numberOfFailedMoves = 0;

			if (pedagogicalLevel == 0f) // TODO for testing only each round increment Level. Remove later!
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
				currentLevel = (Level) Mathf.Clamp((int) Mathf.Floor(pedagogicalLevel * numberOfLevels),0, numberOfLevels - 1);
			}

			Debug.Log("[Scanner] pedagogicalLevel: " + pedagogicalLevel + " Game Level: " + currentLevel);

			SetLevel(currentLevel);


		}




		private void CreatePoof(Vector3 position, float duration, bool withSound)
		{
			if (withSound) AudioManager.I.PlaySfx(Sfx.BaloonPop);
			GameObject poof = Instantiate(poofPrefab, position, Quaternion.identity) as GameObject;
			Destroy(poof, duration);
		}



		protected override void ReadyForGameplay()
		{
			base.ReadyForGameplay();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnMinigameQuit()
		{
			base.OnMinigameQuit();
		}

		public void CorrectMove(bool roundWon)
		{
			AudioManager.I.PlayDialog("comment_welldone");

			if (roundWon) 
			{			
//				dancingDotsLL.ShowRainbow();
				StartCoroutine(PoofOthers(suitcases));
				StartCoroutine(RoundWon());
			}
			else
			{
//				More needed to be done
			}
		}

		IEnumerator PoofOthers(ScannerSuitcase[] draggables)
		{
			foreach (ScannerSuitcase ss in draggables)
			{
				if (ss.gameObject.activeSelf)
				{
					yield return new WaitForSeconds(0.25f);
					ss.gameObject.SetActive(false);
					CreatePoof(ss.transform.position, 2f, true);
				}

			}
		}

		public void WrongMove(Vector3 pos)
		{
			numberOfFailedMoves++;

			if (numberOfFailedMoves >= allowedFailedMoves)
			{
				StartCoroutine(RoundLost());
			}

		}

		IEnumerator CheckNewRound()
		{
			if (numberOfRoundsPlayed >= numberOfRounds)
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

			StartCoroutine(PoofOthers(suitcases));

			yield return new WaitForSeconds(1.5f);

			StartCoroutine(CheckNewRound());
		}

		IEnumerator RoundWon()
		{
			numberOfRoundsWon++;

			yield return new WaitForSeconds(0.25f);

			AudioManager.I.PlaySfx(Sfx.Win);
			yield return new WaitForSeconds(1f);

			StartCoroutine(CheckNewRound());
		}

		private void EndGame()
		{
			StartCoroutine(EndGame_Coroutine());
		}

		IEnumerator EndGame_Coroutine()
		{
			isPlaying = false;

			yield return new WaitForSeconds(1f);

			endGameCanvas.gameObject.SetActive(true);

			int numberOfStars = 0;

			if (numberOfRoundsWon <= 0) {
				numberOfStars = 0;
				WidgetSubtitles.I.DisplaySentence("game_result_retry");
			} else if ((float)numberOfRoundsWon / numberOfRounds < 0.5f) {
				numberOfStars = 1;
				WidgetSubtitles.I.DisplaySentence("game_result_fair");
			} else if (numberOfRoundsWon < numberOfRounds) {
				numberOfStars = 2;
				WidgetSubtitles.I.DisplaySentence("game_result_good");
			} else {
				numberOfStars = 3;
				WidgetSubtitles.I.DisplaySentence("game_result_great");
			}

			LoggerEA4S.Log("minigame", "Scanner", "correctWords", numberOfRoundsWon.ToString());
			LoggerEA4S.Log("minigame", "Scanner", "wrongWords", (numberOfRounds - numberOfRoundsWon).ToString());
			LoggerEA4S.Save();

			starFlowers.Show(numberOfStars);
		}

	}



	[Serializable]
	public class DancingDotsGamePlayInfo: AnturaGameplayInfo
	{
		[Tooltip("Play session duration in seconds.")]
		public float PlayTime = 0f;
	}
}
