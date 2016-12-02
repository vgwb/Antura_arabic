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
		
		public static MazeGameManager instance;

        public GameObject characterPrefab;
        public GameObject arrowTargetPrefab;

        public MazeCharacter currentCharacter;
		public HandTutorial currentTutorial;

		public List<GameObject> prefabs;

		public Canvas endGameCanvas;

		

		 
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

        public bool isTutorialMode;
        //for letters:
        public Dictionary<string,int> allLetters;
        void setupIndices()
        {
            allLetters = new Dictionary<string, int>();
            for(int i =0; i < prefabs.Count;++i)
            {
                allLetters.Add(prefabs[i].name, i);
            }
        }

        protected override void Awake()
		{
			base.Awake();
			instance = this;


		}

		public void startGame()
		{
            isTutorialMode = true;
            setupIndices();

            Context.GetAudioManager().PlayMusic(Music.Theme8);

            


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



            //init first letter
            MazeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.Maze_Title,()=> {
                initCurrentLetter();
            });
            

		}

        public void initUI()
        {
            //ui:
            MinigamesUI.Init(MinigamesUIElement.Starbar | MinigamesUIElement.Timer);

            timer.initTimer ();
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

			line.material = new Material(Shader.Find("Antura/Transparent"));
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

        IEnumerator waitAndPerformCallback(float seconds, VoidDelegate init, VoidDelegate callback)
        {
            init();

            yield return new WaitForSeconds(seconds);

            callback();
        }


        public void moveToNext(bool won = false)
		{
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;

            isShowingAntura = false;
            //check if current letter is complete:
            if (currentCharacter.isComplete ()) {

                

                if(!isTutorialMode)
                {
                    correctLetters++;
                    currentLetterIndex++;
                }
                //show message:
                MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Win);
                
                TutorialUI.MarkYes(currentCharacter.transform.position + new Vector3(2, 2, 2), TutorialUI.MarkSize.Huge);
                currentCharacter.celebrate(() => {
                    if (currentLetterIndex == 6)
                    { //round is 6
                        endGame();
                        return;
                    }
                    else {
                        if (isTutorialMode)
                        {
                            isTutorialMode = false;
                            initUI();
                        }


                        roundNumber.text = "#" + (currentLetterIndex + 1);
                        restartCurrentLetter(won);
                    }
                });
              

                //print ("Prefab nbr: " + currentLetterIndex + " / " + prefabs.Count);
                
			} else {
				addLine ();
				currentCharacter.nextPath ();
				currentTutorial.moveToNextPath ();
			}
		}

		public void lostCurrentLetter()
		{
            if (!currentCharacter || currentCharacter.isAppearing || !currentCharacter.gameObject.activeSelf) return;

            if (isTutorialMode)
            {
                hideCracks();

                //remove last line
                if(lines.Count > 0)
                {
                    lines[lines.Count - 1].SetVertexCount(0);
                    lines.RemoveAt(lines.Count - 1);
                }
                
                pointsList.RemoveRange(0, pointsList.Count);

                //removeLines();

                TutorialUI.Clear(false);
                addLine();

                currentCharacter.resetToCurrent();
                showCurrentTutorial();
                return;
            }

            wrongLetters++;
			currentLetterIndex++;
			if (currentLetterIndex == 6) {
                endGame();
				return;
			} else {
				roundNumber.text = "#" + (currentLetterIndex + 1);

                MazeConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Lose);
                restartCurrentLetter ();
			}
			
		}

		public void restartCurrentLetter(bool won = false)
		{

            //Destroy (currentPrefab);
            int numberOfStars = 0;
            if (correctLetters == 6)
            {
                numberOfStars = 3;
            }
            else if (correctLetters >= 3)
            {
                numberOfStars = 2;
            }
            else if (correctLetters >= 2)
            {
                numberOfStars = 1;
            }
            else {
                numberOfStars = 0;
            }

            if (numberOfStars > 0)
            {
                MinigamesUI.Starbar.GotoStar(numberOfStars-1);
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
            
		}

		void hideCracks()
		{
			health = 4;
			//hide cracks:
			foreach (Transform child in cracks.transform) {
				child.gameObject.SetActive (false);
			}
		}
        private LL_LetterData currentLL = null;
		void initCurrentLetter()
		{
            currentCharacter = null;
            currentTutorial = null;

            TutorialUI.Clear(false);
            addLine ();


            //get a new letter:
            IQuestionPack newQuestionPack = MazeConfiguration.Instance.Questions.GetNextQuestion();
            List<ILivingLetterData> ldList =  (List < ILivingLetterData > )newQuestionPack.GetCorrectAnswers();
            LL_LetterData ld = (LL_LetterData)ldList[0];
            int index = -1;

            if (allLetters.ContainsKey(ld.Id))
                index = allLetters[ld.Id];
            if (index == -1)
            {
                Debug.Log("Letter got from Teacher is: " + ld.Id + " - does not match 11 models we have, we will play sound of the returned data");
                index = UnityEngine.Random.Range(0, prefabs.Count);
            }
            currentLL = ld;
            currentPrefab = (GameObject)Instantiate(prefabs[index]);

            /*int index = allLetters.IndexOf(ld.Id);

            int found = -1;
            for(int i =0; i < prefabs.Count; ++i)
            {
                if(prefabs[i].GetComponent<MazeLetterBuilder>().letterDataIndex == index)
                {
                    found = i;
                    
                    break;
                }
            }
            
            */


            //currentPrefab.GetComponent<MazeLetterBuilder>().letterData = ld;
            currentPrefab.GetComponent<MazeLetterBuilder>().build(() => {

                if(!isTutorialMode)
                    MazeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(ld);
                


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
            if(isTutorialMode)
            {
                MazeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.Maze_Intro,
                        () => {
                            MazeConfiguration.Instance.Context.GetAudioManager().PlayDialogue(Db.LocalizationDataId.Maze_Tuto, ()=> {
                                MazeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(currentLL);
                            });
                            currentCharacter.initialPosition = currentCharacter.transform.position;
                            currentCharacter.initialRotation = currentCharacter.transform.rotation;
                            currentCharacter.transform.position = new Vector3(0,0,15);
                            currentCharacter.gameObject.SetActive(true);
                            currentCharacter.appear();
                        }
                        );
                return;
            }
            currentCharacter.initialPosition = currentCharacter.transform.position;
            currentCharacter.initialRotation = currentCharacter.transform.rotation;
            currentCharacter.transform.position = new Vector3(0, 0, 15);
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

        public void fixLine()
        {
            lines[lines.Count - 1].material.color = new Color(1,0.54f,0);
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
            if (correctLetters == 6)
            {
                numberOfStars = 3;
            }
            else if (correctLetters >= 3)
            {
                numberOfStars = 2;
            }
            else if (correctLetters >= 2)
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
            if (correctLetters == 6)
            {
                numberOfStars = 3;
            }
            else if (correctLetters >= 3)
            {
                numberOfStars = 2;
            }
            else if (correctLetters >= 2)
            {
                numberOfStars = 1;
            }
            else {
                numberOfStars = 0;
            }
            EndGame(numberOfStars, correctLetters);
            
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
