using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


using EA4S;

namespace EA4S.Scanner
{

	public class ScannerGame : MiniGame 
	{

        //		public static ScannerGame instance;

        public static bool disableInput;
        public const string TAG_BELT = "Scanner_Belt";
		public const string TAG_SCAN_START = "Scanner_ScanStart";
		public const string TAG_SCAN_END = "Scanner_ScanEnd";

		public GameObject antura;
		public float anturaMinDelay = 3f;
		public float anturaMaxDelay = 10f;
		public float anturaMinScreenTime = 1f;
		public float anturaMaxScreenTime = 2f;

		public GameObject poofPrefab;

		public ScannerDevice scannerDevice;

		public string currentWord = "";

		[Range(0,1)] public float pedagogicalLevel = 0;

		public int numberOfRounds = 6;

		public int allowedFailedMoves = 3;

		public float maxPlaySpeed;
		public float minPlaySpeed;

		public GameObject LLPrefab;

		[HideInInspector]
		public List<ScannerLivingLetter> scannerLL;

		public List<ScannerSuitcase> suitcases;

		[HideInInspector]
		public List <ILivingLetterData> wordData;

		[HideInInspector]
		public ScannerRoundsManager roundsManager;

		[HideInInspector]
		public int LLCount;

		public int CurrentScoreRecord;

		public Animator trapDoor;

        public ScannerTutorial tut;

        public int STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD;

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
			CurrentScoreRecord = 0;
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
			
			STARS_1_THRESHOLD = numberOfRounds/3;
			STARS_2_THRESHOLD = numberOfRounds/2;
			STARS_3_THRESHOLD = numberOfRounds;

			LLCount = ScannerConfiguration.Instance.nCorrect;

			if (LLCount == 3)
			{
				suitcases.First().gameObject.SetActive(false);
				suitcases.Last().gameObject.SetActive(false);

				suitcases.Remove(suitcases.First());
				suitcases.Remove(suitcases.Last());

				var leftSS = suitcases.First().transform.localPosition;
				suitcases.First().transform.localPosition = new Vector3(-7, leftSS.y, leftSS.z);

				var rightSS = suitcases.Last().transform.localPosition;
				suitcases.Last().transform.localPosition = new Vector3(7, rightSS.y, rightSS.z);

			}


			IntroductionState = new ScannerIntroductionState(this);
			PlayState = new ScannerPlayState(this);
			ResultState = new ScannerResultState(this);

			roundsManager = new ScannerRoundsManager(this);

            tut = GetComponent<ScannerTutorial>();

			//Context.GetOverlayWidget().Initialize(true, false, false);
			Context.GetOverlayWidget().SetStarsThresholds(STARS_1_THRESHOLD, STARS_2_THRESHOLD, STARS_3_THRESHOLD);
		}

		public void PlayWord(float deltaTime, ScannerLivingLetter LL)
		{
			Debug.Log("Play word: " + deltaTime);
			IAudioSource wordSound = Context.GetAudioManager().PlayLetterData(LL.letterObjectView.Data, true);
			wordSound.Pitch = Mathf.Abs(maxPlaySpeed - Mathf.Clamp(deltaTime,minPlaySpeed,maxPlaySpeed + minPlaySpeed));
		}

		public void CreatePoof(Vector3 position, float duration, bool withSound)
		{
			if (withSound) AudioManager.I.PlaySfx(Sfx.BalloonPop);
			GameObject poof = Instantiate(poofPrefab, position, Quaternion.identity) as GameObject;
			poof.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
			Destroy(poof, duration);
		}

//        public override Vector3 GetGravity()
//        {
//            return Vector3.up * (-80);
//        }


    }
}
