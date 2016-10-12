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

namespace EA4S.DancingDots
{
	public enum Diacritic { Sokoun, Fatha, Dameh, Kasrah };

	public class DancingDotsGameManager : MiniGameBase {

		public static DancingDotsGameManager instance;

		public DancingDotsGamePlayInfo GameplayInfo;

		public Canvas endGameCanvas;

		public StarFlowers starFlowers;

//		public Animator dotsMenu;
//		public Animator diacriticMenu;

		public DancingDotsLivingLetter livingLetter;
		public GameObject[] diacritics;
		public DancingDotsDiacriticPosition activeDiacritic;

		public string currentLetter = "";
		private int _dotsCount;
		public int dotsCount
		{
			get 
			{
				return _dotsCount;
			}
			set
			{
				_dotsCount = value;
				foreach (DancingDotsDraggableDot dd in dragableDots)
				{
					dd.isNeeded = dd.dots == _dotsCount;
				}
			}
		}

		public GameObject splatPrefab;

		[Range(0, 255)] public byte dotHintAlpha = 60;
		[Range(0, 255)] public byte diacriticHintAlpha = 60;

		public float hintDotDuration = 2.5f;
		public float hintDiacriticDuration = 3f;

		public GameObject poofPrefab;

		[Range(0,1)] public float pedagogicalLevel = 0;

		public int numberOfRounds = 6;

		public int allowedFailedMoves = 3;

		public DancingDotsDraggableDot[] dragableDots;
		public DancingDotsDraggableDot[] dragableDiacritics;
		public DancingDotsDropZone[] dropZones;

		private bool isCorrectDot = false;
		private bool isCorrectDiacritic = false;

		private int numberOfRoundsWon = 0;
		private int numberOfRoundsPlayed = 0;
		private int numberOfFailedMoves = 0;

		enum Level { Level1, Level2, Level3, Level4, Level5, Level6 };

		private Level currentLevel = Level.Level4;

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
//			StartCoroutine(ShowMenu(dotsMenu));

		}


		private void SetLevel(Level level)
		{
			foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.gameObject.SetActive(false);
			foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.gameObject.SetActive(false);
			foreach (GameObject go in diacritics) go.SetActive(false);

			switch (level)
			{
			case Level.Level1 : // Dots alone with visual aid
//				allowedFailedMoves = 2;
				isCorrectDot = false;
				isCorrectDiacritic = true;
				foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
				StartCoroutine(RemoveHintDot());
				break;

			case Level.Level2 : // Diacritics alone with visual aid
				isCorrectDot = true;
				isCorrectDiacritic = false;
				foreach (DancingDotsDraggableDot dDots in dragableDiacritics) dDots.Reset();
				livingLetter.fullTextGO.SetActive(true); // Show dot
				StartCoroutine(RandomDiacritic());
				StartCoroutine(RemoveHintDiacritic());
				break;

			case Level.Level3 : // Dots and diacritics with visual aid
				isCorrectDot = false;
				isCorrectDiacritic = false;
				foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
				foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.Reset();
				StartCoroutine(RandomDiacritic());
				StartCoroutine(RemoveHintDot());
				StartCoroutine(RemoveHintDiacritic());
				break;

			case Level.Level4 : // Dots alone without visual aid
//				allowedFailedMoves = 2;
				isCorrectDot = false;
				isCorrectDiacritic = true;
				foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
				livingLetter.HideText(livingLetter.hintText);
				break;

			case Level.Level5 : // Diacritics alone without visual aid
				isCorrectDot = true;
				isCorrectDiacritic = false;
				foreach (DancingDotsDraggableDot dDots in dragableDiacritics) dDots.Reset();
				livingLetter.fullTextGO.SetActive(true); // Show dot
				StartCoroutine(RandomDiacritic());
				// Level checked in RandomDiacritic instead of activeDiacritic.Hide(); due to frame delay
				break;

			case Level.Level6 : // Dots and diacritics without visual aid
				isCorrectDot = false;
				isCorrectDiacritic = false;
				foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();
				foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.Reset();
				StartCoroutine(RandomDiacritic());
				livingLetter.HideText(livingLetter.hintText);
				// Level checked in RandomDiacritic instead of activeDiacritic.Hide(); due to frame delay
				break;

			default:
				SetLevel(Level.Level1);
				break;

			}
		}

		private void StartRound()
		{
			
			numberOfRoundsPlayed++;
			Debug.Log("[Dancing Dots] Round: " + numberOfRoundsPlayed);
			numberOfFailedMoves = 0;
			livingLetter.Reset();

			if (pedagogicalLevel == 0f) // TODO for testing only each round increment Level. Remove later!
			{
				currentLevel = (Level) numberOfRoundsPlayed - 1;
			}
			else
			{
				// TODO Move later to Start method
				var numberOfLevels = Enum.GetNames(typeof(Level)).Length;
				currentLevel = (Level) Mathf.Clamp((int) Mathf.Floor(pedagogicalLevel * numberOfLevels),0, numberOfLevels - 1);
			}

			Debug.Log("[Dancing Dots] pedagogicalLevel: " + pedagogicalLevel + " Game Level: " + currentLevel);

			SetLevel(currentLevel);


		}
			

		private void CreatePoof(Vector3 position, float duration)
		{
			AudioManager.I.PlaySfx(Sfx.BaloonPop);
			GameObject poof = Instantiate(poofPrefab, position, Quaternion.identity) as GameObject;
			Destroy(poof, duration);
		}


		IEnumerator RemoveHintDot()
		{
			yield return new WaitForSeconds(hintDotDuration);
			if (!isCorrectDot)
			{
				// find dot postion
				Vector3 poofPosition = Vector3.zero;
				foreach (DancingDotsDropZone dz in dropZones)
				{
					if (dz.letters.Contains(currentLetter))
					{
						poofPosition = new Vector3(dz.transform.position.x, dz.transform.position.y, -8);
						break;
					}
				}
				CreatePoof(poofPosition, 2f);
				livingLetter.HideText(livingLetter.hintText);
			}
		}

		IEnumerator RemoveHintDiacritic()
		{
			yield return new WaitForSeconds(hintDiacriticDuration);
			if (!isCorrectDiacritic)
			{
				CreatePoof(activeDiacritic.transform.position, 2f);
				activeDiacritic.Hide();
			}
		}


//		IEnumerator ShowMenu(Animator animator)
//		{
//			yield return new WaitForSeconds(0.5f);
//			animator.SetTrigger("Show");
//		}
//
//		IEnumerator SwitchMenus(Animator menu1, Animator menu2)
//		{
//			yield return new WaitForSeconds(0.5f);
//			menu1.SetTrigger("Hide");
//			menu2.SetTrigger("Show");
//		}

		private IEnumerator RandomDiacritic() {


			foreach (GameObject go in diacritics)
			{
				go.SetActive(false);
				go.GetComponent<DancingDotsDiacriticPosition>().Hide();
			}

			int random = UnityEngine.Random.Range(0, diacritics.Length);

			activeDiacritic = diacritics[random].GetComponent<DancingDotsDiacriticPosition>();
			activeDiacritic.gameObject.SetActive(true);

			foreach (DancingDotsDraggableDot dd in dragableDiacritics)
			{
				dd.isNeeded = activeDiacritic.diacritic == dd.diacritic;
			}

			// wait for end of frame to ge correct values for meshes
			yield return new WaitForEndOfFrame();
			activeDiacritic.CheckPosition();

			// Level checked in RandomDiacritic instead of SetLevel due to frame delay
			if (currentLevel != Level.Level5 && currentLevel != Level.Level6)
			{
				activeDiacritic.Show();
			}

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

		IEnumerator CorrectMove(bool isWon)
		{
			livingLetter.ShowRainbow();
			yield return new WaitForSeconds(0.25f);
			livingLetter.LivingLetterAnimator.Play("run");

			if (isWon) 
			{
				StartCoroutine(RoundWon());
			}
			else
			{
				yield return new WaitForSeconds(0.5f);
				livingLetter.LivingLetterAnimator.Play("walk");
				livingLetter.HideRainbow();
			}
		}

		IEnumerator PoofOthers(DancingDotsDraggableDot[] draggables)
		{
			foreach (DancingDotsDraggableDot dd in draggables)
			{
				if (dd.gameObject.activeSelf)
				{
					yield return new WaitForSeconds(0.25f);
					dd.gameObject.SetActive(false);
					CreatePoof(dd.transform.position, 2f);
				}

			}
		}

		public void CorrectDot()
		{
			// Change font or show correct character
			isCorrectDot = true;
			livingLetter.fullTextGO.SetActive(true);
			StartCoroutine(PoofOthers(dragableDots));
			StartCoroutine(CorrectMove(isCorrectDiacritic));
		}

		public void CorrectDiacritic()
		{
			isCorrectDiacritic = true;
			activeDiacritic.GetComponent<TextMeshPro>().color = new Color32(0,0,0,255);
			StartCoroutine(PoofOthers(dragableDiacritics));
			StartCoroutine(CorrectMove(isCorrectDot));
		}

		public void WrongMove(Vector3 pos)
		{
			numberOfFailedMoves++;
			Instantiate(splatPrefab,new Vector3(pos.x,pos.y,-20), Quaternion.identity);
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
				livingLetter.GetComponent<Animator>().Play("Pirouette");
				yield return new WaitForSeconds(0.25f);
				livingLetter.HideRainbow();
				StartRound();
				yield return new WaitForSeconds(0.75f);
				livingLetter.LivingLetterAnimator.Play("walk");
			}
		}

		IEnumerator RoundLost()
		{
			yield return new WaitForSeconds(0.5f);
			AudioManager.I.PlaySfx(Sfx.Lose);
			livingLetter.LivingLetterAnimator.Play("ninja");
			StartCoroutine(PoofOthers(dragableDots));
			StartCoroutine(PoofOthers(dragableDiacritics));
			livingLetter.GetComponent<Animator>().Play("FallAndStand");
			yield return new WaitForSeconds(1.5f);

			StartCoroutine(CheckNewRound());
		}

		IEnumerator RoundWon()
		{
			numberOfRoundsWon++;
			AudioManager.I.PlaySfx(Sfx.Win);
			yield return new WaitForSeconds(1f);
			livingLetter.LivingLetterAnimator.Play("ninja");
			yield return new WaitForSeconds(1.5f);

			StartCoroutine(CheckNewRound());
		}

		private void EndGame()
		{
			StartCoroutine(EndGame_Coroutine());
		}

	    IEnumerator EndGame_Coroutine()
		{
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

			LoggerEA4S.Log("minigame", "DancingDots", "correctLetters", numberOfRoundsWon.ToString());
			LoggerEA4S.Log("minigame", "DancingDots", "wrongLetters", (numberOfRounds - numberOfRoundsWon).ToString());
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
