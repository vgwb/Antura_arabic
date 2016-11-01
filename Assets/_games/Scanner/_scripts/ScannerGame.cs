using System;
using TMPro;
using UnityEngine;

using System.Collections;


using EA4S;

namespace EA4S.Scanner
{

	public class ScannerGame : MiniGame {

		public const string TAG_BELT = "Scanner_Belt";
		public const string TAG_SCAN_START = "Scanner_ScanStart";
		public const string TAG_SCAN_END = "Scanner_ScanEnd";

		public GameObject antura;
		public float anturaMinDelay = 3f;
		public float anturaMaxDelay = 10f;
		public float anturaMinScreenTime = 1f;
		public float anturaMaxScreenTime = 2f;

		public GameObject poofPrefab;


		public string currentWord = "";

		[Range(0,1)] public float pedagogicalLevel = 0;

		public int numberOfRounds = 6;

		public int allowedFailedMoves = 3;

		public float maxPlaySpeed;
		public float minPlaySpeed;

		public LetterObjectView letterObjectView;

		public ScannerLivingLetter scannerLL;

		public ScannerSuitcase[] suitcases;

		[HideInInspector]
		public bool isTimesUp;

		[HideInInspector]
		public LL_WordData wordData;

		public int CurrentScore { get; private set; }
		public int CurrentScoreRecord { get; private set; }

		const int STARS_1_THRESHOLD = 5;
		const int STARS_2_THRESHOLD = 8;
		const int STARS_3_THRESHOLD = 12;

		public int CurrentStars
		{
			get
			{
				if (CurrentScoreRecord < STARS_1_THRESHOLD)
					return 0;
				if (CurrentScoreRecord < STARS_2_THRESHOLD)
					return 1;
				if (CurrentScoreRecord < STARS_3_THRESHOLD)
					return 2;
				return 3;
			}
		}

		public ScannerIntroductionState IntroductionState { get; private set; }
		public ScannerPlayState PlayState { get; private set; }
		public ScannerResultState ResultState { get; private set; }

		public void ResetScore()
		{
			CurrentScoreRecord = 10;
			CurrentScore = 10;
		}

		protected override IGameState GetInitialState()
		{
			return IntroductionState;
		}

		protected override IGameConfiguration GetConfiguration()
		{
			return ScannerConfiguration.Instance;
		}

		protected override void OnInitialize(IGameContext context)
		{

			IntroductionState = new ScannerIntroductionState(this);
			PlayState = new ScannerPlayState(this);
			ResultState = new ScannerResultState(this);

//			Physics.gravity = new Vector3(0, -80, 0);
		}

		public void PlayWord(float deltaTime)
		{
			Debug.Log(deltaTime);
			IAudioSource wordSound = Context.GetAudioManager().PlayLetterData(wordData, true);
			wordSound.Pitch = Mathf.Abs(maxPlaySpeed - Mathf.Clamp(deltaTime,minPlaySpeed,maxPlaySpeed + minPlaySpeed));
		}

		public void CreatePoof(Vector3 position, float duration, bool withSound)
		{
			if (withSound) AudioManager.I.PlaySfx(Sfx.BaloonPop);
			GameObject poof = Instantiate(poofPrefab, position, Quaternion.identity) as GameObject;
			Destroy(poof, duration);
		}

//		IEnumerator AnimateAntura()
//		{
//			yield return new WaitForSeconds(1f);
//			//			Vector3 pos = antura.transform.position;
//			// Move antura off screen because SetActive is reseting the animation to running
//			//			antura.transform.position = new Vector3 (-50,pos.y,pos.z);
//			do
//			{
//				yield return new WaitForSeconds(UnityEngine.Random.Range(anturaMinDelay, anturaMaxDelay));
//				//				CreatePoof(pos, 2f, false);
//				yield return new WaitForSeconds(0.4f);
//				//				antura.transform.position = pos;
//				//
//				yield return new WaitForSeconds(UnityEngine.Random.Range(anturaMinScreenTime, anturaMaxScreenTime));
//				//				CreatePoof(pos, 2f, false);
//				//				antura.transform.position = new Vector3 (-50,pos.y,pos.z);
//			} while (isPlaying);
//
//		}
	}

//
//
//	[Serializable]
//	public class DancingDotsGamePlayInfo: AnturaGameplayInfo
//	{
//		[Tooltip("Play session duration in seconds.")]
//		public float PlayTime = 0f;
//	}
}
