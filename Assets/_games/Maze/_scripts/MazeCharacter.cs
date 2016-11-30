using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace EA4S.Maze
{
    public delegate void VoidDelegate();
    public class MazeCharacter: MonoBehaviour {

		//for internal use:
		


		public List<Vector3> characterWayPoints;

        public LetterObjectView LL;

		public GameObject myCollider;
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


		public List<GameObject> _fruits;
		int currentFruitList = 0;

	


		int currentFruitIndex;

		private bool startCheckingForCollision = false;
		private bool donotHandleBorderCollision = false;
        public bool isFleeing = false;
        private Vector3 fleePosition;

        public bool isAppearing = false;
        public GameObject rocket;

        private GameObject blinkingTarget;

		void Start()
		{
            LL.SetState(LLAnimationStates.LL_rocketing);
           
            isFleeing = false;
            characterIsMoving = false;
			characterWayPoints = new List<Vector3>();
			currentCharacterWayPoint = 0;



			currentWayPoint = 0;
			GetComponent<Collider> ().enabled = false;

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
           
            var dir = _fruits[1].transform.position - transform.position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            angle = 360 + angle;

            
            
            transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(-angle, Vector3.up), 0.5f);

            //transform.position += new Vector3(0, 0.05f, 0);
            dir.Normalize();
            dir.x = transform.position.x - dir.x*1.5f;
            dir.z = transform.position.z - dir.z * 1.5f;
            dir.y = 1;
            transform.DOMove(dir, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
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

                //if(count > 0)
                //	child.gameObject.GetComponent<Renderer> ().material.color = Color.red;

                if (child.gameObject.GetComponent<MazeArrow>() == null)
                    child.gameObject.AddComponent<MazeArrow>();
                else
                    child.gameObject.GetComponent<MazeArrow>().resetColor();

                if (count == 0) {
					child.gameObject.GetComponent<MazeArrow> ().pingPong = true;

                    if(blinkingTarget == null)
                    {
                        blinkingTarget = (GameObject)Instantiate(MazeGameManager.instance.arrowTargetPrefab);
                        blinkingTarget.transform.position = child.transform.position;
                        blinkingTarget.transform.localScale = new Vector3(2, 2, 2);
                        blinkingTarget.transform.SetParent(transform.parent, true);
                    }
                    
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

                    if(index == 0)
                    {
                        if(blinkingTarget != null)
                        {
                            Destroy(blinkingTarget);
                            blinkingTarget = null;
                        }
                    }


				}/* else if(index > currentFruitIndex){
					//lose?
					waitAndRestartScene();
				}*/
			}
		}


		void OnTriggerExit(Collider other)
		{
            
			print ("trigger exit " + other.gameObject.name);
			print ("Current letter " + MazeGameManager.instance.currentPrefab.name);

			if (other.gameObject.name == "MazeLetter") {
				//if the character completely exits the maze letter:
				//stop for a second and restart the level:
				waitAndRestartScene();
			}
		
		}

		void wasHit()
		{
			

			MazeGameManager.instance.wasHit ();




			if (MazeGameManager.instance.health == 0) {

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
                transform.DOPause();
			},
				()=>{
					characterIsMoving = true;
                    transform.DOPlay();

                }));
			

		}



		void waitAndRestartScene()
		{
            //if (particles) particles.SetActive(false);
            foreach (GameObject particle in particles) particle.SetActive(false);
            //stop for a second and restart the level:
            StartCoroutine(waitAndPerformCallback(3,()=>{
				MazeGameManager.instance.showAllCracks();
				donotHandleBorderCollision = true;
				characterIsMoving = false;
                transform.DOKill(false);
                //launchRocket = true;
                toggleVisibility(true);

                if(!MazeGameManager.instance.isTutorialMode)
                {
                    LL.transform.SetParent(transform, true);
                    LL.SetState(LLAnimationStates.LL_idle);

                    GameObject obj = new GameObject();
                    obj.transform.position = rocket.transform.position - new Vector3(10,0, 0);
                    obj.transform.SetParent(rocket.transform.parent, true);
                    rocket.transform.SetParent(obj.transform, true);
                    rocket.transform.DOLookAt(Camera.main.transform.position, 0.5f, AxisConstraint.None, Vector3.forward);
                    rocket.transform.DOMove(Camera.main.transform.position + new Vector3(10, 10, 0),5 );

                    obj.transform.DOLocalRotate(new Vector3(0, 180, 0), 4);
                    
                }
                
            },
				()=>{
					MazeGameManager.instance.lostCurrentLetter();//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            toggleVisibility(false);
            MazeGameManager.instance.moveToNext (true);
		}

		public void nextPath()
		{
			if (currentFruitList == Fruits.Count - 1)
				return;

            transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isInside = false;
            currentFruitList++;




			setFruitsList ();


            Vector3 initPos = _fruits[0].transform.position + new Vector3(0, 1,0);

            initialPosition = initPos;
			targetPos = initialPosition;

			initialRotation = transform.rotation;
			targetRotation = initialRotation;

			currentCharacterWayPoint = 0;
			characterWayPoints = new List<Vector3> ();
			characterWayPoints.Add (initialPosition);


            var dir = (_fruits[0].transform.position) - transform.position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(-angle, Vector3.up), 0.25f);


            toggleVisibility(true);
            transform.DOMove(_fruits[0].transform.position, 1).OnComplete(() => {
                toggleVisibility(false);
                dir = _fruits[1].transform.position - transform.position;
                angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
                transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(-angle, Vector3.up), 0.5f);

                dir.Normalize();
                dir.x = transform.position.x - dir.x* 1.5f;
                dir.z = transform.position.z - dir.z * 1.5f;
                dir.y = 1;
                transform.DOMove(dir, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });
            


        }


        public void resetToCurrent()
        {
            transform.DOKill(false);
            donotHandleBorderCollision = false;
            transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isInside = false;
            transform.position = _fruits[0].transform.position + new Vector3(0, 1,0);


            initialPosition = transform.position;
            targetPos = initialPosition;

            initialRotation = transform.rotation;
            targetRotation = initialRotation;

            currentCharacterWayPoint = 0;
            characterWayPoints = new List<Vector3>();
            characterWayPoints.Add(initialPosition);


            setFruitsList();

            toggleVisibility(false);

            var dir = _fruits[1].transform.position - transform.position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            angle = 360 + angle;



            transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(-angle, Vector3.up), 0.5f);


            dir.Normalize();
            dir.x = transform.position.x - dir.x * 1.5f;
            dir.z = transform.position.z - dir.z * 1.5f;
            dir.y = 1;
            transform.DOMove(dir, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            characterIsMoving = false;
            GetComponent<Collider>().enabled = false;
        }

		public bool canMouseBeDown()
		{
            if (_fruits == null || MazeGameManager.instance.isShowingAntura) return false;

			if (_fruits.Count == 0)
				return false;
			
			float distance =  Camera.main.transform.position.y;
			Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(distance));
			pos = Camera.main.ScreenToWorldPoint(pos);

			//check distance to first fruit:
			//pos.z = _fruits[0].transform.position.z;

            float mag = (pos - _fruits[0].transform.position).sqrMagnitude;
           
            return ((pos - _fruits [0].transform.position).sqrMagnitude) <= 1;


		}

        private void moveTween()
        {
            /*if (currentCharacterWayPoint + 3 < characterWayPoints.Count)
            {
                var dir = transform.position - characterWayPoints[currentCharacterWayPoint + 3];
                var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

                targetRotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;
                transform.DORotateQuaternion(targetRotation, 0.007f);
            }

            transform.DOMove(characterWayPoints[currentCharacterWayPoint], 0.007f).OnComplete(moveTweenComplete);*/

            //average distance:
            float distance = 0;
            for (int i = 1; i < characterWayPoints.Count; ++i)
                distance += (characterWayPoints[i] - characterWayPoints[i - 1]).sqrMagnitude;

            float time = distance * 2;
            if (time > 2) time = 2;
            transform.DOPath(characterWayPoints.ToArray(), time, PathType.Linear, PathMode.Ignore).OnWaypointChange((int index) => {
                if (index + 3 < characterWayPoints.Count)
                {
                    var dir = transform.position - characterWayPoints[index + 3];
                    var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;
                   // transform.DORotateQuaternion(targetRotation, 0.007f);
                }
            }).OnComplete(pathMoveComplete);
        }

        private void pathMoveComplete()
        {
            transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isInside = false;

            //arrived!
            //transform.rotation = initialRotation;
            if (currentFruitIndex == _fruits.Count)
            {

                print("Won");
                // if (particles) particles.SetActive(false);
                foreach (GameObject particle in particles) particle.SetActive(false);
                GetComponent<Collider>().enabled = false;
                characterIsMoving = false;
                toggleVisibility(false);
                transform.DOKill(false);
                MazeGameManager.instance.moveToNext(true);

                if (currentFruitList == Fruits.Count - 1)
                {
                    if (dot != null)
                        dot.GetComponent<BoxCollider>().enabled = true;
                }
            }
            else
                waitAndRestartScene();
        }

        private void moveTweenComplete()
        {
            if (currentCharacterWayPoint < characterWayPoints.Count - 1)
            {
                currentCharacterWayPoint++;
                //reached the end:
                if (currentCharacterWayPoint == characterWayPoints.Count - 1)
                {

                    transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isInside = false;

                    //arrived!
                    //transform.rotation = initialRotation;
                    if (currentFruitIndex == _fruits.Count)
                    {

                        print("Won");
                        // if (particles) particles.SetActive(false);
                        foreach (GameObject particle in particles) particle.SetActive(false);
                        GetComponent<Collider>().enabled = false;
                        characterIsMoving = false;
                        transform.DOKill(false);
                        toggleVisibility(false);
                        MazeGameManager.instance.moveToNext(true);

                        if (currentFruitList == Fruits.Count - 1)
                        {
                            if (dot != null)
                                dot.GetComponent<BoxCollider>().enabled = true;
                        }
                    }
                    else
                        waitAndRestartScene();
                }
                else
                    moveTween();

                //enable collider when we reach the second waypoint
                if (currentCharacterWayPoint == 1)
                    myCollider.SetActive(true);
            }
        }
		public void initMovement()
		{
            if (characterIsMoving) return;

            MazeGameManager.instance.fixLine();

            transform.DOKill(false);
            characterIsMoving = true;
			GetComponent<Collider> ().enabled = true;
           // if (particles) particles.SetActive(true);
            foreach (GameObject particle in particles) particle.SetActive(true);
            foreach (GameObject fruit in _fruits) {
				fruit.GetComponent<BoxCollider> ().enabled = true;
			}

            //test with tweens:
            moveTween();
            
            /*
            transform.position = Vector3.MoveTowards (transform.position, characterWayPoints[currentCharacterWayPoint], Time.deltaTime*10);

				if (currentCharacterWayPoint + 3 < characterWayPoints.Count) {
					var dir = transform.position - characterWayPoints [currentCharacterWayPoint + 3];
					var angle = Mathf.Atan2 (dir.z, dir.x) * Mathf.Rad2Deg;

                    targetRotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;

					transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 5);
				}
				
				if(transform.position == characterWayPoints[currentCharacterWayPoint] && currentCharacterWayPoint < characterWayPoints.Count-1){

					currentCharacterWayPoint++;

					//reached the end:
					if (currentCharacterWayPoint == characterWayPoints.Count-1) {

                        transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isInside = false;

                        //arrived!
                        //transform.rotation = initialRotation;
                        if (currentFruitIndex == _fruits.Count) {
                            
                            print ("Won");
                           // if (particles) particles.SetActive(false);
                            foreach (GameObject particle in particles) particle.SetActive(false);
                            GetComponent<Collider> ().enabled = false;
							characterIsMoving = false;
							MazeGameManager.instance.moveToNext (true);

							if (currentFruitList == Fruits.Count - 1) {
								if (dot != null)
									dot.GetComponent<BoxCollider> ().enabled = true;
							}
						} else
							waitAndRestartScene ();
					}

					//enable collider when we reach the second waypoint
					if (currentCharacterWayPoint == 1)
						myCollider.SetActive (true);
				}*/
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
				MazeGameManager.instance.appendToLine(targetPos);
			}





			var dir = transform.position - characterWayPoints[currentCharacterWayPoint];
			

			if(previousPosition != targetPos)
			{
                

                characterWayPoints.Add(targetPos + new Vector3(0,1,0));

			}

			if ((_fruits [_fruits.Count - 1].transform.position - targetPos).sqrMagnitude < 0.1f) {

				toggleVisibility (true);
				initMovement ();
				MazeGameManager.instance.timer.StopTimer ();
			}

		}

        public void appear()
        {
            toggleVisibility(true);
            isAppearing = true;

            List<Vector3> pts = new List<Vector3>();
            pts.Add(transform.position);

            Vector3 half = transform.position + (initialPosition - transform.position ) / 2;
            half.x += 10;

            pts.Add(half);

            pts.Add(initialPosition);

            transform.DOPath(pts.ToArray(), 2, PathType.CatmullRom, PathMode.Ignore).OnWaypointChange((int index) => {
                if (index + 1 < pts.Count)
                {
                    var dir = transform.position - pts[index + 1];
                    var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;
                                                                                  // transform.DORotateQuaternion(targetRotation, 0.007f);
                }
            }).OnComplete(()=> {
                toggleVisibility(false);
                isAppearing = false;



                //transform.rotation = initialRotation;
                MazeGameManager.instance.showCurrentTutorial();
            });

        }

        public void celebrate(System.Action action)
        {
            toggleVisibility(true);
            Vector3 pos = transform.position + new Vector3(10, 0, 20);
            List<Vector3> pts = new List<Vector3>();
            pts.Add(transform.position);

            Vector3 vec = pos - transform.position;
            Vector3 perp = Vector3.Cross(vec, Vector3.up);

            Vector3 half = transform.position + (vec) / 2;

            half += perp.normalized * 3;

            pts.Add(half);

            pts.Add(pos);

            transform.DOPath(pts.ToArray(), 2.5f, PathType.CatmullRom, PathMode.Ignore).OnWaypointChange((int index) => {
                if (index + 1 < pts.Count)
                {
                    var dir = transform.position - pts[index + 1];
                    var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;
                                                                                  // transform.DORotateQuaternion(targetRotation, 0.007f);
                }
            }).OnComplete(() => {
                toggleVisibility(false);
                gameObject.SetActive(false);
                action();
            });
        }

        public void fleeTo(Vector3 position)
        {
            //wait and flee:
            StartCoroutine(waitAndPerformCallback(0.5f, () => {
                
            },
                () => {
                    transform.DOKill(true);
                    toggleVisibility(true);
                    fleePosition = position;
                    isFleeing = true;
                }));


            
        }

        
        void moveTowards(Vector3 position, float speed = 10, bool useUpVector = true)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

            var dir = transform.position - position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            targetRotation = Quaternion.AngleAxis(-angle, useUpVector? Vector3.up: Vector3.forward);// * initialRotation;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5);
        }

		void Update()
		{
            /*if(launchRocket)
            {
                //moveTowards(Camera.main.transform.position,20, false);
               
                return;
            }*/
			if(isAppearing)
            {
               /* moveTowards(initialPosition);
                if (transform.position == initialPosition)
                {
                    toggleVisibility(false);
                    isAppearing = false;

                    

                    //transform.rotation = initialRotation;
                    MazeGameManager.instance.showCurrentTutorial();
                }*/
                return;
            }
            if(isFleeing)
            {

                moveTowards(fleePosition);

                if (transform.position == fleePosition)
                {
                    //wait then show cracks:
                    StartCoroutine(waitAndPerformCallback(3, () => {
                        MazeGameManager.instance.showAllCracks();
                        donotHandleBorderCollision = true;
                        characterIsMoving = false;
                        transform.DOKill(false);
                    },
                    () => {
                        MazeGameManager.instance.lostCurrentLetter();
                    }));


                }
                return;
            }


			/*if (characterIsMoving) {
				transform.position = Vector3.MoveTowards (transform.position, characterWayPoints[currentCharacterWayPoint], Time.deltaTime*10);

				if (currentCharacterWayPoint + 3 < characterWayPoints.Count) {
					var dir = transform.position - characterWayPoints [currentCharacterWayPoint + 3];
					var angle = Mathf.Atan2 (dir.z, dir.x) * Mathf.Rad2Deg;

                    targetRotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;

					transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 5);
				}
				
				if(transform.position == characterWayPoints[currentCharacterWayPoint] && currentCharacterWayPoint < characterWayPoints.Count-1){

					currentCharacterWayPoint++;

					//reached the end:
					if (currentCharacterWayPoint == characterWayPoints.Count-1) {

                        transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isInside = false;

                        //arrived!
                        //transform.rotation = initialRotation;
                        if (currentFruitIndex == _fruits.Count) {
                            
                            print ("Won");
                           // if (particles) particles.SetActive(false);
                            foreach (GameObject particle in particles) particle.SetActive(false);
                            GetComponent<Collider> ().enabled = false;
							characterIsMoving = false;
							MazeGameManager.instance.moveToNext (true);

							if (currentFruitList == Fruits.Count - 1) {
								if (dot != null)
									dot.GetComponent<BoxCollider> ().enabled = true;
							}
						} else
							waitAndRestartScene ();
					}

					//enable collider when we reach the second waypoint
					if (currentCharacterWayPoint == 1)
						myCollider.SetActive (true);
				}
			}*/
		}
	}
}