using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

namespace EA4S.Scanner
{
	public class ScannerRoundsManager {
		
		public event Action <int> onRoundsFinished;

		IAudioSource wordAudioSource;

		private int numberOfRoundsWon = 0;
		private int numberOfRoundsPlayed = 0;
		private int numberOfFailedMoves = 0;

		ScannerGame game;
		bool initialized = false;
		string lastWordDataId = "";


		enum Level { Level1, Level2, Level3, Level4, Level5, Level6 };

		private Level currentLevel = Level.Level4;

		public ScannerRoundsManager(ScannerGame game)
		{
			this.game = game;
		}

		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;

				StartRound();

				foreach (ScannerSuitcase ss in game.suitcases)
				{
					ss.onCorrectDrop += CorrectMove;
					ss.onWrongDrop += WrongMove;
				}
				game.scannerLL = GameObject.Instantiate(game.LLPrefab).GetComponent<ScannerLivingLetter>();
				game.scannerLL.onReset += OnLetterReset;
				game.scannerLL.onFallOff += OnLetterFallOff;
				game.scannerLL.onStartFallOff += OnLetterStartFallOff;
				game.scannerLL.onPassedMidPoint += OnLetterPassedMidPoint;
//				game.pipesAnswerController.Initialize(game);
//				CreateQuestionLivingLetters();
//				questionLetterIndex = livingLetters.Count - 1;
//				nextQuestionTimer = 0f;
//				requestNextQueston = false;
//				game.Context.GetInputManager().onPointerDown += OnPointerDown;
//				game.Context.GetInputManager().onPointerUp += OnPointerUp;
//				game.Context.GetInputManager().onPointerDrag += OnPointerDrag;
			}
		}

		private void OnLetterPassedMidPoint()
		{
			// Decide if Antura will bark
			// Antura leaves
			// Trapdoor drops
		}

		private void OnLetterReset()
		{
			do
			{
				game.wordData = AppManager.Instance.Teacher.GetRandomTestWordDataLL();
			} while (game.wordData.Data.Id == lastWordDataId);
			lastWordDataId = game.wordData.Data.Id;
			game.scannerLL.letterObjectView.Init(game.wordData);
			SetupSuitCases();
		}


		private void SetupSuitCases()
		{
			List <String> chosenWords = new List<String>();
			chosenWords.Add(game.wordData.Data.Id);

			int correctOne = UnityEngine.Random.Range(0,game.suitcases.Length);
			for (int i = 0; i < game.suitcases.Length; i++)
			{
				ScannerSuitcase ss = game.suitcases[i];
				ss.Reset();
				if (i == correctOne)
				{
					Debug.Log(game.wordData.TextForLivingLetter);
					ss.drawing.text = game.wordData.DrawingCharForLivingLetter;
					ss.isCorrectAnswer = true;
				}
				else
				{
					LL_WordData wrongWord;
					do
					{
						wrongWord = AppManager.Instance.Teacher.GetRandomTestWordDataLL();
					} while (chosenWords.Contains(wrongWord.Data.Id));
					chosenWords.Add(wrongWord.Data.Id);
					Debug.Log(wrongWord.TextForLivingLetter);
					ss.drawing.text = wrongWord.DrawingCharForLivingLetter;
					ss.isCorrectAnswer = false;
				}
			}
		}

		private void StartRound()
		{

			numberOfRoundsPlayed++;
			numberOfFailedMoves = 0;

			if (ScannerConfiguration.Instance.Difficulty == 0f) // TODO for testing only each round increment Level. Remove later!
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

		public void CorrectMove(GameObject GO)
		{
			AudioManager.I.PlayDialog("comment_welldone");
			// TODO Drop suitcase next to LL
			game.StartCoroutine(PoofOthers(game.suitcases));
			game.StartCoroutine(RoundWon());

		}

		public void WrongMove(GameObject GO)
		{
			numberOfFailedMoves++;
			game.CreatePoof(GO.transform.position,2f,true);
			GO.SetActive(false);

			if (numberOfFailedMoves >= game.allowedFailedMoves)
			{
				game.StartCoroutine(RoundLost());
			}

		}

		private void CheckNewRound()
		{
			if (numberOfRoundsPlayed >= game.numberOfRounds)
			{
				onRoundsFinished(numberOfRoundsWon);
			}
			else
			{
				StartRound();
			}
		}

		private void OnLetterStartFallOff()
		{
			AudioManager.I.PlaySfx(Sfx.Lose);
			game.StartCoroutine(PoofOthers(game.suitcases));
		}


		private void OnLetterFallOff()
		{
			CheckNewRound();
		}

		IEnumerator RoundLost()
		{
			yield return new WaitForSeconds(0.5f);
			AudioManager.I.PlaySfx(Sfx.Lose);
			game.scannerLL.RoundLost();
			game.StartCoroutine(PoofOthers(game.suitcases));
			CheckNewRound();
		}

		IEnumerator RoundWon()
		{
			numberOfRoundsWon++;

			game.Context.GetOverlayWidget().SetStarsScore(numberOfRoundsWon);

			yield return new WaitForSeconds(0.25f);
			AudioManager.I.PlaySfx(Sfx.Win);
			game.scannerLL.RoundWon();
			CheckNewRound();
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
			//TODO Different levels
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
