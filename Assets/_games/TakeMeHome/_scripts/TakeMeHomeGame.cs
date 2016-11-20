using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EA4S.TakeMeHome
{
	public class TakeMeHomeGame : MiniGame
	{
		
		public TextMeshProUGUI timerText;
		public TextMeshProUGUI roundText;
		public GameObject tubes;
		public GameObject spawnTube;
		public Transform LLSpawnPosition;
		public GameObject antura;

		[HideInInspector]
		public TakeMeHomeLL currentLetter;

		[HideInInspector]
		public int currentTube = 0;

		private int _activeTubes = 0;

		public int CurrentScore { get; private set; }


		[HideInInspector]
		public bool isTimesUp;


		[HideInInspector]
		public List<GameObject> activeTubes;

        [HideInInspector]
        public List<GameObject> allTubes;

        [HideInInspector]
		public TakeMeHomeLetterManager letterManager;

		[HideInInspector]
		public int currentRound;

		[HideInInspector]
		//public CountdownTimer gameTime;

		int stars1Threshold
		{
			get
			{
				return 2;
			}
		}

		int stars2Threshold
		{
			get
			{
				return 4;
			}
		}

		int stars3Threshold
		{
			get
			{
				return 6;
			}
		}


		public int CurrentStars
		{
			get
			{
				if (CurrentScore < stars1Threshold)
					return 0;
				if (CurrentScore < stars2Threshold)
					return 1;
				if (CurrentScore < stars3Threshold)
					return 2;
				return 3;
			}
		}

		public TakeMeHomeIntroductionState IntroductionState { get; private set; }
		public TakeMeHomePlayState PlayState { get; private set; }
		public TakeMeHomeResultState ResultState { get; private set; }
		public TakeMeHomeEndState EndState { get; private set; }
		public TakeMeHomeResetState ResetState { get; private set; }
		public TakeMeHomeAnturaState AntureState { get; private set; }
        public TakeMeHomeTutorialIntroState TutorialIntroState { get; private set; }
        public TakeMeHomeTutorialPlayState TutorialPlayState { get; private set; }
        public TakeMeHomeTutorialResetState TutorialResetState { get; private set; }


        public void InitTubes()
		{
			letterManager = GetComponent<TakeMeHomeLetterManager> ();
			activeTubes = new List<GameObject> ();
            allTubes = new List<GameObject>();

            foreach (Transform child in tubes.transform) {
				child.gameObject.name = "tube_" + allTubes.Count;
				child.gameObject.SetActive (false);
                allTubes.Add (child.gameObject);

			}

		}


		public void activateTubes(int count = 2)
		{
            if (count < 2) count = 2;

			_activeTubes = count;
			for (int i = 0; i < count; ++i)
            {
                int index = UnityEngine.Random.Range(0, allTubes.Count);
                activeTubes.Add(allTubes[index]);
                allTubes[index].SetActive(true);

                allTubes.RemoveAt(index);
            }
				
		}


		public void ResetScore()
		{
			CurrentScore = 0;
		}

		public void IncrementScore()
		{
            
			++CurrentScore;
            //update stars:
            int stars = CurrentStars;
            if (stars > 0)
                MinigamesUI.Starbar.GotoStar(stars - 1);
        }

		public void IncrementRound()
		{
			currentLetter = null;
			++currentRound;
			letterManager.removeLetter ();

			if (currentRound > 6) {
				
				return;
			}

			/*if (currentRound <= 2)
				activateTubes (2);
			else if (currentRound <= 4)
				activateTubes (3);
			else
				activateTubes (4);*/
			

			roundText.text = "#"+currentRound.ToString ();

			spawnLetteAtTube ();
		}

		public void spawnLetteAtTube()
		{
			
			currentTube = UnityEngine.Random.Range(0,_activeTubes);

            int tubeIndex = int.Parse(activeTubes[currentTube].name.Substring(5));

            currentTube = tubeIndex;
             currentLetter = letterManager.spawnLetter (AppManager.Instance.Teacher.GetAllTestLetterDataLL()[TakeMeHomeModel.Instance.getRandomLetterOnTube(tubeIndex)]);
			currentLetter.MoveBy (new UnityEngine.Vector3 (-11, 0, 0),1.8f);
		}

		 

		protected override IGameConfiguration GetConfiguration()
		{
			return TakeMeHomeConfiguration.Instance;
		}

		protected override IGameState GetInitialState()
		{
			return TutorialIntroState;
		}

		protected override void OnInitialize(IGameContext context)
		{
            //float difficulty = FastCrowdConfiguration.Instance.Difficulty;
            TutorialIntroState = new TakeMeHomeTutorialIntroState(this);
            TutorialPlayState = new TakeMeHomeTutorialPlayState(this);
            TutorialResetState = new TakeMeHomeTutorialResetState(this);
            IntroductionState = new TakeMeHomeIntroductionState(this);
			PlayState = new TakeMeHomePlayState(this);
			ResultState = new TakeMeHomeResultState(this);
			EndState = new TakeMeHomeEndState(this);
			ResetState = new TakeMeHomeResetState (this);
			AntureState = new TakeMeHomeAnturaState (this);

			timerText.gameObject.SetActive(false);
			roundText.gameObject.SetActive(false);
			Physics.gravity = new Vector3(0, -10, 0);


			Physics.gravity = Vector3.up * -40;

			InitTubes ();


			//setup timer and round info:
			currentRound = 0;
			roundText.text = "#"+currentRound.ToString ();

			Context.GetAudioManager().PlayMusic(Music.Theme3);


            //add antura specific script:
            antura.AddComponent<TakeMeHomeAntura>();

            

            /*gameTime = new CountdownTimer(UnityEngine.Mathf.Lerp(90.0f, 60.0f, TakeMeHomeConfiguration.Instance.Difficulty));
			gameTime.onTimesUp += OnTimesUp;

			gameTime.Reset();*/
            isTimesUp = false;

            


            activateTubes(UnityEngine.Mathf.RoundToInt(TakeMeHomeConfiguration.Instance.Difficulty * 4));
        }

        public void initUI()
        {
            //ui:
            MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer);
            MinigamesUI.Timer.Setup( UnityEngine.Mathf.Lerp(90.0f, 60.0f, TakeMeHomeConfiguration.Instance.Difficulty));
        }


        public void followLetter()
		{
			if (!currentLetter)
				return;

			//currentLetter.panicAndRun ();

			antura.SetActive (true);
            

			antura.GetComponent<TakeMeHomeAntura> ().SetAnturaTime (true, new Vector3(8.4f, -3.44f, -15));
		}


		public void OnTimesUp()
		{
            if (isTimesUp) return;

			// Time's up!
			isTimesUp = true;

			this.SetCurrentState(EndState);
		}
	}
}