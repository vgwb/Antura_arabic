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
		

		public int CurrentScore { get; private set; }


		[HideInInspector]
		public bool isTimesUp;


		[HideInInspector]
		public List<GameObject> activeTubes;

		[HideInInspector]
		public TakeMeHomeLetterManager letterManager;

		[HideInInspector]
		public int currentRound;

		int stars1Threshold
		{
			get
			{
				return 3;
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
				return 5;
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

		public void InitTubes(int count = 2)
		{
			letterManager = GetComponent<TakeMeHomeLetterManager> ();
			activeTubes = new List<GameObject> ();
			int c = 0;
			foreach (Transform child in tubes.transform) {
				child.gameObject.SetActive (false);
				activeTubes.Add (child.gameObject);
			}
		}


		public void activateTubes(int count = 2)
		{
			for (int i = 0; i < count; ++i)
				activeTubes [i].SetActive (true);
		}


		public void ResetScore()
		{
			CurrentScore = 0;
		}

		public void IncrementScore()
		{
			++CurrentScore;
		}

		public void IncrementRound()
		{
			++currentRound;
			letterManager.removeLetter ();

			if (currentRound > 6) {
				
				return;
			}

			if (currentRound <= 2)
				activateTubes (2);
			else if (currentRound <= 4)
				activateTubes (3);
			else
				activateTubes (4);
			

			roundText.text = "#"+currentRound.ToString ();

			letterManager.spawnLetter (TakeMeHomeConfiguration.Instance.Letters.GetNextData ());
		}

		protected override IGameConfiguration GetConfiguration()
		{
			return TakeMeHomeConfiguration.Instance;
		}

		protected override IGameState GetInitialState()
		{
			return IntroductionState;
		}

		protected override void OnInitialize(IGameContext context)
		{
			//float difficulty = FastCrowdConfiguration.Instance.Difficulty;

			IntroductionState = new TakeMeHomeIntroductionState(this);
			PlayState = new TakeMeHomePlayState(this);
			ResultState = new TakeMeHomeResultState(this);
			EndState = new TakeMeHomeEndState(this);

			timerText.gameObject.SetActive(false);
			roundText.gameObject.SetActive(false);
			Physics.gravity = new Vector3(0, -10, 0);


			Physics.gravity = Vector3.up * -40;



		}



	}
}