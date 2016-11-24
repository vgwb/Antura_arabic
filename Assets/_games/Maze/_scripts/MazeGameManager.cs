using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;
using TMPro;


namespace EA4S.Maze
{
	public class MazeGameManager : MiniGame
    {
		
		public static MazeGameManager Instance;

        public GameObject characterPrefab;

		/*public MazeGameplayInfo GameplayInfo;*/

		public MazeCharacter currentCharacter;
		public HandTutorial currentTutorial;

		public List<GameObject> prefabs;

		public Canvas endGameCanvas;

		public StarFlowers starFlowers;



		 
		public float idleTime = 7;
		public TextMeshProUGUI roundNumber;


		int currentLetterIndex;
		public GameObject currentPrefab;
		public int health = 4;
		public GameObject cracks;
		List<GameObject> _cracks;
		//List<GameObject> lines;
		public List<Vector3> pointsList;

		public List<LineRenderer> lines;

		int correctLetters = 0;
		int wrongLetters = 0;

		[HideInInspector]
		public float gameTime = 0;
		public float maxGameTime = 120;
		public MazeTimer timer;
        public GameObject antura;
        public GameObject fleePositionObject;

        private List<Vector3> fleePositions;

        protected override void Awake()
		{
			base.Awake();
			Instance = this;


		}

		public void startGame()
		{
            //base.Start();

            /*	AppManager.Instance.InitDataAI();
                AppManager.Instance.CurrentGameManagerGO = gameObject;
                SceneTransitioner.Close();*/

            fleePositions = new List<Vector3>();
            foreach (Transform child in fleePositionObject.transform)
            {
                fleePositions.Add(child.position);
            }

            antura.AddComponent<MazeAntura>();
            //cracks to display:
            _cracks = new List<GameObject> ();
			cracks.SetActive (true);
			foreach (Transform child in cracks.transform) {
				child.gameObject.SetActive (false);
				_cracks.Add (child.gameObject);
			}
			//lines = new List<GameObject>();

			lines = new List<LineRenderer> ();




			currentLetterIndex = 0;
			roundNumber.text = "#" + (currentLetterIndex + 1);

			gameTime = maxGameTime / (1 + MazeConfiguration.Instance.Difficulty);

            //ui:
            MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer);

            timer.initTimer ();

			//init first letter
			initCurrentLetter();

		}

		public void addLine()
		{
			
			pointsList = new List<Vector3> ();
			GameObject go = new GameObject ();
			go.transform.position = new Vector3 (0, 0, -0.2f);
            go.transform.Rotate(new Vector3(90,0,0));
            LineRenderer line = go.AddComponent<LineRenderer> ();
			//line.material = new Material (Shader.Find ("Particles/Additive"));
			line.SetVertexCount (0);
			line.SetWidth (0.6f, 0.6f);
			//line.SetColors (Color.green, Color.green);
			//line.useWorldSpace = true;    

			line.material = new Material(Shader.Find("Unlit/TransparentColor"));
			line.material.color = new Color (0.5f, 0.5f, 0.5f, 0.5f);

			lines.Add (line);

		}

		/*protected override void ReadyForGameplay()
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
		}*/

		public bool tutorialForLetterisComplete()
		{
			return currentTutorial.isCurrentTutorialDone ();
		}

		public bool isCurrentLetterComplete()
		{
			return currentTutorial.isComplete ();
		}

		public void showAllCracks()
		{
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;
            if (health == 0)
				return;
			
			for (int i = 0; i < _cracks.Count; ++i)
				_cracks [i].SetActive (true);
			//StartCoroutine (shakeCamera (0.5f, 0.5f));

		}
		public void wasHit()
		{

            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;
            _cracks [_cracks.Count- health].SetActive (true);
			health--;

			//StartCoroutine (shakeCamera (0.5f, 0.5f));

		}

		public void moveToNext(bool won = false)
		{
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;

            isShowingAntura = false;
            //check if current letter is complete:
            if (currentCharacter.isComplete ()) {
				correctLetters++;
				currentLetterIndex++;
				print ("Prefab nbr: " + currentLetterIndex + " / " + prefabs.Count);
				if (currentLetterIndex == prefabs.Count) {
                    endGame();
					return;
				} else {
					roundNumber.text = "#" + (currentLetterIndex + 1);
					restartCurrentLetter (won);
				}
			} else {
				addLine ();
				currentCharacter.nextPath ();
				currentTutorial.moveToNextPath ();
			}
		}

		public void lostCurrentLetter()
		{
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;

            wrongLetters++;
			currentLetterIndex++;
			if (currentLetterIndex == prefabs.Count) {
                endGame();
				return;
			} else {
				roundNumber.text = "#" + (currentLetterIndex + 1);
                
				restartCurrentLetter ();
			}
			
		}

		public void restartCurrentLetter(bool won = false)
		{
            
            //Destroy (currentPrefab);
            int numberOfStars = 0;
            if (correctLetters == prefabs.Count)
            {
                numberOfStars = 3;
            }
            else if (correctLetters > prefabs.Count / 2)
            {
                numberOfStars = 2;
            }
            else if (correctLetters > prefabs.Count / 4)
            {
                numberOfStars = 1;
            }
            else {
                numberOfStars = 0;
            }

            if(numberOfStars > 0)
            {
                MinigamesUI.Starbar.GotoStar(numberOfStars-1);
            }

            //show message:
            if (won)
				AudioManager.I.PlaySfx (Sfx.Win);
			else
            {
                AudioManager.I.PlaySfx(Sfx.Lose);
                
            }
				

			currentPrefab.SendMessage("moveOut",won);

			hideCracks ();
			removeLines ();

			initCurrentLetter ();
		




		}

		void removeLines()
		{
			foreach(LineRenderer line in lines)		
				line.SetVertexCount (0);
			lines = new List<LineRenderer> ();
			pointsList.RemoveRange (0, pointsList.Count);

			/*foreach (GameObject line in lines)
				Destroy (line);
			lines = new List<GameObject>();*/
		}

		void hideCracks()
		{
			health = 4;
			//hide cracks:
			foreach (Transform child in cracks.transform) {
				child.gameObject.SetActive (false);
			}
		}

		void initCurrentLetter()
		{
            currentCharacter = null;
            currentTutorial = null;

            TutorialUI.Clear(false);
            addLine ();
			currentPrefab = (GameObject)Instantiate(prefabs[currentLetterIndex]);

            currentPrefab.GetComponent<MazeLetterBuilder>().build(() => {
                foreach (Transform child in currentPrefab.transform)
                {
                    if (child.name == "Mazecharacter")
                        currentCharacter = child.GetComponent<MazeCharacter>();
                    else if (child.name == "HandTutorial")
                        currentTutorial = child.GetComponent<HandTutorial>();
                }

                currentCharacter.gameObject.SetActive(false);
            });

        }

        public void showCharacterMovingIn()
        {
            currentCharacter.initialPosition = currentCharacter.transform.position;
            currentCharacter.initialRotation = currentCharacter.transform.rotation;
            currentCharacter.transform.position = getRandFleePosition();
            currentCharacter.gameObject.SetActive(true);
            currentCharacter.appear();
        }

		public void showCurrentTutorial()
		{
            isShowingAntura = false;

            if (currentTutorial != null) {
				currentTutorial.showCurrentTutorial ();

			}
			if (currentCharacter != null) {
                
                currentCharacter.initialize ();

			}

		}

		IEnumerator shakeCamera(float duration, float magnitude) {
			
			float elapsed = 0.0f;

			Vector3 originalCamPos = Camera.main.transform.position;

			while (elapsed < duration) {

				elapsed += Time.deltaTime;          

				float percentComplete = elapsed / duration;         
				float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

				// map value to [-1, 1]
				float x = UnityEngine.Random.value * 2.0f - 1.0f;
				float y = UnityEngine.Random.value * 2.0f - 1.0f;
				x *= magnitude * damper;
				y *= magnitude * damper;

				Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);

				yield return null;
			}

			Camera.main.transform.position = originalCamPos;
		}

		public void appendToLine(Vector3 mousePos)
		{
			if (!pointsList.Contains (mousePos)) {
				//mousePos.z = -0.1071415f;
				pointsList.Add (mousePos);
				lines[lines.Count-1].SetVertexCount (pointsList.Count);
				lines[lines.Count-1].SetPosition (pointsList.Count - 1, (Vector3)pointsList [pointsList.Count - 1]);
			}
		}
		public void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			/*
			start.z = end.z = -0.1f;//-0.1f;
			GameObject myLine = new GameObject();
			myLine.transform.position = start;
			myLine.AddComponent<LineRenderer>();
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = new Material(Shader.Find("Unlit/Color"));
			lr.material.color = color;
			//lr.SetColors(color, color);

			lr.SetWidth(0.3f, 0.3f);
			lr.SetPosition(0, start);
			lr.SetPosition(1, end);

			lines.Add(myLine);*/
		}

        bool gameEnded = false;
        private void endGame()
		{
            if (gameEnded)
                return;

            gameEnded = true;

            MinigamesUI.Timer.Pause();
            TutorialUI.Clear(false);

            int numberOfStars = 0;
            if (correctLetters == prefabs.Count)
            {
                numberOfStars = 3;
            }
            else if (correctLetters > prefabs.Count / 2)
            {
                numberOfStars = 2;
            }
            else if (correctLetters > prefabs.Count / 4)
            {
                numberOfStars = 1;
            }
            else {
                numberOfStars = 0;
            }

            if (numberOfStars > 0)
            {
                MinigamesUI.Starbar.GotoStar(numberOfStars - 1);
            }

            EndGame(numberOfStars, correctLetters);
            //StartCoroutine(EndGame_Coroutine());
        }

        

        private IEnumerator EndGame_Coroutine()
		{
			yield return new WaitForSeconds(1f);
            int numberOfStars = 0;
            if (correctLetters == prefabs.Count)
            {
                numberOfStars = 3;
            }
            else if (correctLetters > prefabs.Count / 2)
            {
                numberOfStars = 2;
            }
            else if (correctLetters > prefabs.Count / 4)
            {
                numberOfStars = 1;
            }
            else {
                numberOfStars = 0;
            }
            EndGame(numberOfStars, correctLetters);
            /*endGameCanvas.gameObject.SetActive(true);

			int numberOfStars = 0;

			if (correctLetters == prefabs.Count) {
				numberOfStars = 3;
				WidgetSubtitles.I.DisplaySentence ("game_result_great");
			} else if (correctLetters > prefabs.Count / 2) {
				numberOfStars = 2;
				WidgetSubtitles.I.DisplaySentence ("game_result_good");
			} else if (correctLetters > prefabs.Count / 4) {
				numberOfStars = 1;
				WidgetSubtitles.I.DisplaySentence ("game_result_fair");
			} else {
				numberOfStars = 0;
				WidgetSubtitles.I.DisplaySentence("game_result_retry");
			}

           

			LoggerEA4S.Log("minigame", "Maze", "correctLetters", ""+correctLetters);
			LoggerEA4S.Log("minigame", "Maze", "wrongLetters", ""+wrongLetters);
			LoggerEA4S.Save();

			starFlowers.Show(numberOfStars);*/
        }


        public void onTimeUp()
		{
            //end game:
            endGame();
		}

        public bool isShowingAntura = false;
		public void onIdleTime()
		{
            if (isShowingAntura) return;
            isShowingAntura = true;

            timer.StopTimer();

            antura.SetActive (true);
            antura.GetComponent<MazeAntura>().SetAnturaTime(true,currentCharacter.transform.position);

            int randIndex = UnityEngine.Random.Range(0, fleePositions.Count);
            currentCharacter.fleeTo(fleePositions[randIndex]);
        }

        public Vector3 getRandFleePosition()
        {
            int randIndex = UnityEngine.Random.Range(0, fleePositions.Count);
            return (fleePositions[randIndex]);
        }

        //states
        public MazeIntroState IntroductionState { get; private set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return MazeConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            
            IntroductionState = new MazeIntroState(this);
             

        }
    }
/*
	[Serializable]
	public class MazeGameplayInfo : AnturaGameplayInfo
	{
		[Tooltip("Play session duration in seconds.")]
		public float PlayTime = 0f;
	}*/
}
