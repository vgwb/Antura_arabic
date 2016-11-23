using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	
	public class MazeCharacter: MonoBehaviour {

		//for internal use:
		private delegate void VoidDelegate();


		public List<Vector3> characterWayPoints;

        public LetterObjectView LL;

		public GameObject collider;
        public List<GameObject> particles;

		public List<GameObject> Fruits;


		public bool characterIsMoving;

		public MazeDot dot = null;

		public Transform nextPosition;


		int currentCharacterWayPoint;


		public Vector3 initialPosition;
		public Quaternion initialRotation;
		Vector3 targetPos;
		Quaternion targetRotation;
		int currentWayPoint;


		List<GameObject> _fruits;
		int currentFruitList = 0;

	


		int currentFruitIndex;

		private bool startCheckingForCollision = false;
		private bool donotHandleBorderCollision = false;
        public bool isFleeing = false;
        private Vector3 fleePosition;

        public bool isAppearing = false;

		void Start()
		{
            LL.SetState(LLAnimationStates.LL_rocketing);
           
            isFleeing = false;
            characterIsMoving = false;
			characterWayPoints = new List<Vector3>();
			currentCharacterWayPoint = 0;



			currentWayPoint = 0;
			GetComponent<BoxCollider> ().enabled = false;

			//collider.GetComponent<MeshRenderer> ().enabled = false;
			//collider.SetActive(false);


            //foreach (GameObject fruitList in Fruits)
            //	fruitList.SetActive (false);


              

        }

		public void toggleVisibility(bool value) {
            foreach(GameObject particle in particles) particle.SetActive(value);
            // toggles the visibility of this gameobject and all it's children
            /*Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer r in renderers)
				r.enabled = value;*/
        }

		public void initialize()
		{
			initialPosition = transform.position;
			targetPos = initialPosition;

			initialRotation = transform.rotation;
			targetRotation = initialRotation;


			characterWayPoints.Add(initialPosition);
			setFruitsList ();
           // if (particles) particles.SetActive(false);
          /*  var dir = transform.position - _fruits[0].transform.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation =  Quaternion.AngleAxis(angle, Vector3.forward);*/


		}


		void setFruitsList()
		{
			//fruits to collect
			_fruits = new List<GameObject> ();
			int i = 0;
			if (Fruits.Count == 0)
				return;
			
			//Fruits [currentFruitList].SetActive (true);
			int count = 0;

			foreach (Transform child in Fruits[currentFruitList].transform) {
				//child.gameObject.SetActive (i==0||i==1? true:false);

				if(count > 0)
					child.gameObject.GetComponent<Renderer> ().material.color = Color.red;

				child.gameObject.AddComponent<MazeArrow> ();
				if (count == 0) {
					//child.gameObject.transform.localScale = new Vector3 (3, 3, 3);
					child.gameObject.GetComponent<MazeArrow> ().pingPong = true;
				}

				child.gameObject.name = "fruit_" + (i++);
				child.gameObject.GetComponent<BoxCollider> ().enabled = false;
				_fruits.Add (child.gameObject);
				++count;
			}
			currentFruitIndex = 0;
		}
		

		void OnTriggerEnter(Collider other)
		{
			if (donotHandleBorderCollision || !characterIsMoving)
				return;
			
			print ("Colliding with: " + other.gameObject.name);

			if (other.gameObject.name == "BorderCollider") {

				//if this is the 1st hit ignore it:
				if (!startCheckingForCollision) {
					startCheckingForCollision = true;
					return;
				}

				wasHit ();
			}

			if (other.gameObject.name.IndexOf ("fruit_") == 0) {
				//we hit a fruit make sure it is in order:
				int index = int.Parse( other.gameObject.name.Substring(6));

				if (index == currentFruitIndex) {
					_fruits [currentFruitIndex].GetComponent<BoxCollider> ().enabled = false;

					//lerp
					_fruits [currentFruitIndex].GetComponent<MazeArrow> ().pingPong = false;
					_fruits [currentFruitIndex].GetComponent<MazeArrow> ().tweenToColor = true;

					//_fruits [currentFruitIndex].SetActive (false);
					currentFruitIndex++;



				} else if(index > currentFruitIndex){
					//lose?
					waitAndRestartScene();
				}
			}
		}


		void OnTriggerExit(Collider other)
		{
            
			print ("trigger exit " + other.gameObject.name);
			print ("Current letter " + MazeGameManager.Instance.currentPrefab.name);

			if (other.gameObject.name == "MazeLetter") {
				//if the character completely exits the maze letter:
				//stop for a second and restart the level:
				waitAndRestartScene();
			}
		
		}

		void wasHit()
		{
			

			MazeGameManager.Instance.wasHit ();




			if (MazeGameManager.Instance.health == 0) {

				waitAndRestartScene ();

				return;
			}

		
			//stop checking for border collision for half a second
			StartCoroutine(waitAndPerformCallback(0.5f,()=>{
				donotHandleBorderCollision = true;
			},
				()=>{
					donotHandleBorderCollision = false;
				}));

			//stop moving the character for a second
			StartCoroutine(waitAndPerformCallback(1,()=>{
				characterIsMoving = false;
			},
				()=>{
					characterIsMoving = true;
				}));
			

		}



		void waitAndRestartScene()
		{
            //if (particles) particles.SetActive(false);
            foreach (GameObject particle in particles) particle.SetActive(false);
            //stop for a second and restart the level:
            StartCoroutine(waitAndPerformCallback(1,()=>{
				MazeGameManager.Instance.showAllCracks();
				donotHandleBorderCollision = true;
				characterIsMoving = false;
               
            },
				()=>{
					MazeGameManager.Instance.lostCurrentLetter();//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}));


		}

		//corutine to handle pausing a bit then resuming
		IEnumerator waitAndPerformCallback(float seconds,VoidDelegate init, VoidDelegate callback)
		{
			init ();

			yield return new WaitForSeconds (seconds);

			callback ();
		}

		public bool isComplete()
		{
			if (currentFruitList == Fruits.Count - 1) {
				if (dot == null)
					return true;
				else
					return dot.isClicked;
			} else
				return false;
			
		}

		public void setClickedDot()
		{
			MazeGameManager.Instance.moveToNext (true);
		}

		public void nextPath()
		{
			if (currentFruitList == Fruits.Count - 1)
				return;


			currentFruitList++;




			setFruitsList ();
			transform.position = _fruits[0].transform.position;


			initialPosition = transform.position;
			targetPos = initialPosition;

			initialRotation = transform.rotation;
			targetRotation = initialRotation;

			currentCharacterWayPoint = 0;
			characterWayPoints = new List<Vector3> ();
			characterWayPoints.Add (initialPosition);


			/*var dir = transform.position - _fruits[0].transform.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation =  Quaternion.AngleAxis(angle, Vector3.forward);*/

			toggleVisibility (false);
            
		}

		public bool canMouseBeDown()
		{
            if (_fruits == null || MazeGameManager.Instance.isShowingAntura) return false;

			if (_fruits.Count == 0)
				return false;
			
			float distance =  Camera.main.transform.position.y;
			Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(distance));
			pos = Camera.main.ScreenToWorldPoint(pos);

			//check distance to first fruit:
			//pos.z = _fruits[0].transform.position.z;

            float mag = (pos - _fruits[0].transform.position).sqrMagnitude;
            bool b = mag <= 0;
            b = mag <= float.Epsilon;
            return ((pos - _fruits [0].transform.position).sqrMagnitude) <= 1;


		}

		public void initMovement()
		{
			
			characterIsMoving = true;
			GetComponent<BoxCollider> ().enabled = true;
           // if (particles) particles.SetActive(true);
            foreach (GameObject particle in particles) particle.SetActive(true);
            foreach (GameObject fruit in _fruits) {
				fruit.GetComponent<BoxCollider> ().enabled = true;
			}
		}

		public void calculateMovementAndRotation()
		{
			//if(victory) return;

			Vector3 previousPosition = targetPos;
			float distance =  (0.1f) - Camera.main.transform.position.y;
			targetPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -distance);
			targetPos = Camera.main.ScreenToWorldPoint(targetPos);

			if (previousPosition != initialPosition && previousPosition != targetPos) {
				//MazeGameManager.Instance.DrawLine (previousPosition, targetPos, Color.red);
				MazeGameManager.Instance.appendToLine(targetPos);
			}





			var dir = transform.position - characterWayPoints[currentCharacterWayPoint];
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			//targetRotation =  Quaternion.AngleAxis(angle, Vector3.forward) * initialRotation;


			if(previousPosition != targetPos)
			{
				//targetPos.z = (_fruits[_fruits.Count - 1].transform.position;
				characterWayPoints.Add(targetPos);

			}

			//print ((_fruits [_fruits.Count - 1].transform.position - targetPos).sqrMagnitude);
			if ((_fruits [_fruits.Count - 1].transform.position - targetPos).sqrMagnitude < 0.5f) {

				toggleVisibility (true);
				initMovement ();
				MazeGameManager.Instance.timer.StopTimer ();
			}

		}

        public void appear()
        {
            toggleVisibility(true);
            isAppearing = true;
        }
        public void fleeTo(Vector3 position)
        {
            //wait and flee:
            StartCoroutine(waitAndPerformCallback(0.5f, () => {
                
            },
                () => {
                    toggleVisibility(true);
                    fleePosition = position;
                    isFleeing = true;
                }));


            
        }


        void moveTowards(Vector3 position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * 10);

            var dir = transform.position - position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            targetRotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5);
        }

		void Update()
		{
			if(isAppearing)
            {
                moveTowards(initialPosition);
                if (transform.position == initialPosition)
                {
                    toggleVisibility(false);
                    isAppearing = false;
                    //transform.rotation = initialRotation;
                    MazeGameManager.Instance.showCurrentTutorial();
                }
                return;
            }
            if(isFleeing)
            {

                moveTowards(fleePosition);

                if (transform.position == fleePosition)
                {
                    MazeGameManager.Instance.showAllCracks();
                    MazeGameManager.Instance.lostCurrentLetter();

                   
                }
                return;
            }


			if (characterIsMoving) {
				transform.position = Vector3.MoveTowards (transform.position, characterWayPoints[currentCharacterWayPoint], Time.deltaTime*10);

				if (currentCharacterWayPoint + 3 < characterWayPoints.Count) {
					var dir = transform.position - characterWayPoints [currentCharacterWayPoint + 3];
					var angle = Mathf.Atan2 (dir.z, dir.x) * Mathf.Rad2Deg;

                    targetRotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;

					transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 5);
				}
				//transform.LookAt(characterWayPoints[currentCharacterWayPoint+1]);

				if(transform.position == characterWayPoints[currentCharacterWayPoint] && currentCharacterWayPoint < characterWayPoints.Count-1){

					currentCharacterWayPoint++;

					//reached the end:
					if (currentCharacterWayPoint == characterWayPoints.Count-1) {
						//arrived!
						//transform.rotation = initialRotation;
						if (currentFruitIndex == _fruits.Count) {
							print ("Won");
                           // if (particles) particles.SetActive(false);
                            foreach (GameObject particle in particles) particle.SetActive(false);
                            GetComponent<BoxCollider> ().enabled = false;
							characterIsMoving = false;
							MazeGameManager.Instance.moveToNext (true);

							if (currentFruitList == Fruits.Count - 1) {
								if (dot != null)
									dot.GetComponent<BoxCollider> ().enabled = true;
							}
						} else
							waitAndRestartScene ();
					}

					//enable collider when we reach the second waypoint
					if (currentCharacterWayPoint == 1)
						collider.SetActive (true);
				}
			}
		}
	}
}