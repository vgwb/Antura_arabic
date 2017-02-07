using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EA4S.LivingLetters;
using EA4S.Minigames.Tobogan;

namespace EA4S.Minigames.Maze
{
    public delegate void VoidDelegate();
    public class MazeCharacter : MonoBehaviour
    {
        private const float VERTICAL_DISTANCE_FROM_CAMERA = 0.2f;
        private const float MIN_XZ_DISTANCE_FROM_CAMERA = 1f;
        private const float MAX_XZ_DISTANCE_FROM_CAMERA = 2f;

        private const float CELEBRATION_PATH_MIDPOINT_X_ANCHOR = 0.2f;
        private const float CELEBRATION_PATH_MIDPOINT_Z_ANCHOR = -0.2f;

        private const float CELEBRATION_PATH_ENDPOINT_X_ANCHOR = -0.5f;
        private const float CELEBRATION_PATH_ENDPOINT_Z_ANCHOR = 0f;
        private const float CELEBRATION_PATH_ENDPOINT_DISTANCE_FROM_CAMERA = 5.5f;
        private const float CELEBRATION_PATH_DURATION = 2.5f;

        private const float FLEE_PATH_MIDPOINT_X_ANCHOR = -0.6f;
        private const float FLEE_PATH_MIDPOINT_Z_ANCHOR = 0.2f;

        private const float FLEE_PATH_ENDPOINT_X_ANCHOR = 0.33f;
        private const float FLEE_PATH_ENDPOINT_Z_ANCHOR = -0.33f;
        private const float FLEE_PATH_ENDPOINT_DISTANCE_FROM_CAMERA = 5f;
        private const float FLEE_PATH_DURATION = 2.5f;

        private enum LLState
        {
            Normal, Braked, Impacted, Ragdolling
        }

        private LLState _state;
        private LLState State
        {
            get
            {
                return _state;
            }

            set
            {
                if (_state != value)
                {
                    _state = value;

                    switch (_state)
                    {
                        case LLState.Ragdolling:
                            ragdoll.SetRagdoll(true, rocket.GetComponent<Rigidbody>().velocity);

                            foreach (Collider collider in ragdoll.GetComponentsInChildren<Collider>())
                            {
                                collider.enabled = true;
                            }

                            break;
                    }

                    stateTime = 0f;
                }
            }
        }
        private float stateTime;

        public enum LoseState
        {
            None, OutOfBounds, Incomplete
        }

        public LoseState loseState;

        private Vector3 brakeRotation;

        public List<Vector3> characterWayPoints;

        public LetterObjectView LL;
        private Transform LLParent;

        public MeshCollider myCollider;
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

        public List<GameObject> _fruits;

        int currentFruitList = 0;

        int currentFruitIndex;

#pragma warning disable 0219
#pragma warning disable 0414
        private bool startCheckingForCollision = false;
        private bool donotHandleBorderCollision = false;
#pragma warning restore 0414
#pragma warning restore 0219

        public bool isFleeing = false;

        public bool isAppearing = false;
        public GameObject rocket;

        private GameObject blinkingTarget;

        private MazeLetter mazeLetter;

        public GameObject winParticleVFX;

        private readonly Vector3 ROCKET_LOCAL_ROTATION = new Vector3(0, -90f, 90f);

        public LivingLetterRagdoll ragdoll;

        public void SetMazeLetter(MazeLetter mazeLetter)
        {
            this.mazeLetter = mazeLetter;
        }

        void Start()
        {
            LL.SetState(LLAnimationStates.LL_rocketing);

            LLParent = ragdoll.transform.parent;

            isFleeing = false;
            characterIsMoving = false;
            characterWayPoints = new List<Vector3>();
            currentCharacterWayPoint = 0;

            GetComponent<Collider>().enabled = false;

            foreach (Collider collider in rocket.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        private Vector3 GetCorrectedRotationOfRocket(Vector3 direction)
        {
            var xAngle = Vector3.Angle(direction, Vector3.up);
            var yAngle = Vector3.Angle(direction, Vector3.left);
            var zAngle = Vector3.Angle(direction, Vector3.left) - 20;

            return new Vector3(xAngle, yAngle, zAngle);
        }

        private float GetFrustumHeightAtDistance(float distanceFromCamera)
        {
            return 2.0f * distanceFromCamera * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        private float GetFrustumWidth(float frustumHeight)
        {
            return frustumHeight * Camera.main.aspect;
        }

        public void SpawnOffscreen()
        {
            var frustumHeight = GetFrustumHeightAtDistance(VERTICAL_DISTANCE_FROM_CAMERA);
            var frustumWidth = GetFrustumWidth(frustumHeight);

            var xDisplacement = Random.Range(MIN_XZ_DISTANCE_FROM_CAMERA, MAX_XZ_DISTANCE_FROM_CAMERA);
            xDisplacement *= -1f;

            var zDisplacement = Random.Range(MIN_XZ_DISTANCE_FROM_CAMERA, MAX_XZ_DISTANCE_FROM_CAMERA);
            zDisplacement *= -1f;

            var cameraPosition = Camera.main.transform.position;

            Vector3 startPoint = new Vector3(cameraPosition.x + (frustumWidth / 2 * Mathf.Sign(xDisplacement)) + xDisplacement,
                                                cameraPosition.y - VERTICAL_DISTANCE_FROM_CAMERA, cameraPosition.z + (frustumHeight / 2 * Mathf.Sign(zDisplacement)) + zDisplacement);

            transform.position = startPoint;
        }

        public void toggleVisibility(bool value)
        {
            foreach (GameObject particle in particles) particle.SetActive(value);
        }

        private void ResetRocket()
        {
            var rocketRigidBody = rocket.GetComponent<Rigidbody>();
            rocketRigidBody.isKinematic = true;
            rocketRigidBody.useGravity = false;
            rocketRigidBody.velocity = Vector3.zero;
            rocketRigidBody.angularVelocity = Vector3.zero;
            rocket.transform.localPosition = Vector3.zero;
            rocket.transform.localRotation = Quaternion.Euler(ROCKET_LOCAL_ROTATION);

            rocket.GetComponent<SphereCollider>().enabled = false;
        }

        private void ResetLivingLetter()
        {
            ragdoll.transform.parent = LLParent;
            ragdoll.SetRagdoll(false, Vector3.zero);

            ragdoll.transform.localPosition = Vector3.zero;
            ragdoll.transform.localRotation = Quaternion.Euler(Vector3.zero);

            foreach (Collider collider in ragdoll.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            LL.SetState(LLAnimationStates.LL_rocketing);
        }

        private void Reset()
        {
            ResetRocket();
            ResetLivingLetter();

            loseState = LoseState.None;

            myCollider.enabled = true;

            GetComponent<CapsuleCollider>().enabled = true;
        }

        public void initialize()
        {
            Reset();

            initialPosition = transform.position;
            targetPos = initialPosition;

            initialRotation = transform.rotation;
            targetRotation = initialRotation;

            characterWayPoints.Add(initialPosition);
            SetFruitsList();

            var firstArrowRotation = _fruits[0].transform.rotation.eulerAngles;
            firstArrowRotation.x += 90f;
            firstArrowRotation.y += 180f;

            transform.DORotate(firstArrowRotation, 0.5f).OnComplete(() =>
            {
                transform.DOMove(transform.position - transform.TransformVector(Vector3.left), 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });
        }

        public void CreateFruits(List<GameObject> fruitsLists)
        {
            foreach (GameObject fruitsList in fruitsLists)
            {
                for (int i = 0; i < fruitsList.transform.childCount; i++)
                {
                    Transform child = fruitsList.transform.GetChild(i);

                    MazeArrow mazeArrow = child.gameObject.AddComponent<MazeArrow>();
                    mazeArrow.SetMazeLetter(mazeLetter);

                    child.gameObject.name = "fruit_" + (i);
                }
            }

            Fruits = fruitsLists;
        }

        private void SetFruitsList()
        {
            if (Fruits.Count == 0)
            {
                return;
            }

            // Fruits to collect:
            _fruits = new List<GameObject>();

            for (int i = 0; i < Fruits[currentFruitList].transform.childCount; i++)
            {
                GameObject child = Fruits[currentFruitList].transform.GetChild(i).gameObject;
                MazeArrow mazeArrow = child.gameObject.GetComponent<MazeArrow>();
                mazeArrow.Reset();

                if (i == 0)
                {
                    mazeArrow.Highlight(true);
                }

                _fruits.Add(child);
            }

            foreach (GameObject fruit in _fruits)
            {
                fruit.GetComponent<BoxCollider>().enabled = true;
            }

            currentFruitIndex = 1;
        }

        void OnTriggerEnter(Collider other)
        {
            if (donotHandleBorderCollision || !characterIsMoving)
                return;

            print("Colliding with: " + other.gameObject.name);

            if (other.gameObject.name.IndexOf("fruit_") == 0)
            {
                //we hit a fruit make sure it is in order:
                int index = int.Parse(other.gameObject.name.Substring(6));

                if (index == 0)
                {
                    return;
                }

                if (index == currentFruitIndex)
                {
                    //lerp
                    _fruits[currentFruitIndex].GetComponent<MazeArrow>().pingPong = false;
                    _fruits[currentFruitIndex].GetComponent<MazeArrow>().tweenToColor = true;

                    currentFruitIndex++;

                    if (index == 0)
                    {
                        if (blinkingTarget != null)
                        {
                            Destroy(blinkingTarget);
                            blinkingTarget = null;
                        }
                    }
                }
            }
        }

        void waitAndRestartScene()
        {
            //if (particles) particles.SetActive(false);
            foreach (GameObject particle in particles) particle.SetActive(false);
            //stop for a second and restart the level:
            StartCoroutine(waitAndPerformCallback(3, () =>
            {
                donotHandleBorderCollision = true;
                characterIsMoving = false;
                transform.DOKill(false);
                toggleVisibility(true);

                MazeGameManager.instance.ColorCurrentLinesAsIncorrect();

            },
                () =>
                {
                    MazeGameManager.instance.lostCurrentLetter();
                }));
        }

        //corutine to handle pausing a bit then resuming
        IEnumerator waitAndPerformCallback(float seconds, VoidDelegate init, VoidDelegate callback)
        {
            init();

            yield return new WaitForSeconds(seconds);

            callback();
        }

        public bool isComplete()
        {
            if (currentFruitList == Fruits.Count - 1)
            {
                if (dot == null)
                    return true;
                else
                    return dot.isClicked;
            }
            else
                return false;

        }

        public void setClickedDot()
        {
            toggleVisibility(false);
            MazeGameManager.instance.moveToNext(true);
        }

        public void nextPath()
        {
            if (currentFruitList == Fruits.Count - 1)
                return;

            transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isDrawing = false;
            currentFruitList++;

            SetFruitsList();


            Vector3 initPos = _fruits[0].transform.position + new Vector3(0, 1, 0);

            initialPosition = initPos;
            targetPos = initialPosition;

            initialRotation = transform.rotation;
            targetRotation = initialRotation;

            currentCharacterWayPoint = 0;
            characterWayPoints = new List<Vector3>();
            characterWayPoints.Add(initialPosition);


            var dir = (_fruits[0].transform.position) - transform.position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(-angle, Vector3.up), 0.25f);


            toggleVisibility(true);
            transform.DOMove(_fruits[0].transform.position, 1).OnComplete(() =>
            {
                toggleVisibility(false);

                var firstArrowRotation = _fruits[0].transform.rotation.eulerAngles;
                firstArrowRotation.x += 90f;
                firstArrowRotation.y += 180f;

                transform.DORotate(firstArrowRotation, 0.5f).OnComplete(() =>
                {
                    transform.DOMove(transform.position - transform.TransformVector(Vector3.left), 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                });
            });
        }

        public void resetToCurrent()
        {
            transform.DOKill(false);
            donotHandleBorderCollision = false;
            transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isDrawing = false;
            transform.position = _fruits[0].transform.position + new Vector3(0, 1, 0);

            initialPosition = transform.position;
            targetPos = initialPosition;

            initialRotation = transform.rotation;
            targetRotation = initialRotation;

            currentCharacterWayPoint = 0;
            characterWayPoints = new List<Vector3>();
            characterWayPoints.Add(initialPosition);


            SetFruitsList();

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

            float distance = Camera.main.transform.position.y;
            Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(distance));
            pos = Camera.main.ScreenToWorldPoint(pos);

            float mag = (pos - _fruits[0].transform.position).sqrMagnitude;

            return ((pos - _fruits[0].transform.position).sqrMagnitude) <= 4;
        }

        private void MoveTween()
        {
            // Average distance:
            float distance = 0;
            for (int i = 1; i < characterWayPoints.Count; ++i)
                distance += (characterWayPoints[i] - characterWayPoints[i - 1]).sqrMagnitude;

            float time = distance * 2;
            if (time > 2) time = 2;

            if (loseState == LoseState.OutOfBounds)
            {
                time = 0.33f;
            }

            transform.DOPath(characterWayPoints.ToArray(), time, PathType.Linear, PathMode.Ignore).OnWaypointChange((int index) =>
            {
                if (index + 3 < characterWayPoints.Count)
                {
                    var dir = transform.position - characterWayPoints[index + 3];
                    var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);

                }
            }).OnComplete(pathMoveComplete);
        }

        private void pathMoveComplete()
        {
            transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isDrawing = false;

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
            {
                if (loseState != LoseState.OutOfBounds)
                {
                    for (int i = currentFruitIndex; i < _fruits.Count; i++)
                    {
                        _fruits[i].GetComponent<MazeArrow>().MarkAsUnreached(i == currentFruitIndex);
                    }

                    Vector3 direction = _fruits[currentFruitIndex].transform.position - rocket.transform.position;
                    Vector3 rotatedVector = direction;
                    var piOverTwo = Mathf.PI / 2;
                    rotatedVector.x = direction.x * Mathf.Cos(piOverTwo) - direction.z * Mathf.Sin(piOverTwo);
                    rotatedVector.z = direction.x * Mathf.Sin(piOverTwo) + direction.z * Mathf.Cos(piOverTwo);
                    rotatedVector.y = 0f;
                    rotatedVector.Normalize();
                    rotatedVector *= 1.5f;
                    rotatedVector.y = 2f;

                    Tutorial.TutorialUI.MarkNo((_fruits[currentFruitIndex].transform.position + rocket.transform.position) / 2 + rotatedVector, Tutorial.TutorialUI.MarkSize.Normal);

                    loseState = LoseState.Incomplete;
                }

                else
                {
                    OnRocketImpactedWithBorder();
                }

                waitAndRestartScene();
            }
        }

        public void UnhighlightStartingFX()
        {
            _fruits[0].GetComponent<MazeArrow>().Unhighlight();
        }

        private void OnRocketImpactedWithBorder()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            myCollider.enabled = false;
            mazeLetter.GetComponent<BoxCollider>().enabled = false;

            ragdoll.transform.SetParent(rocket.transform, true);

            rocket.GetComponent<SphereCollider>().enabled = true;

            var rocketRigidBody = rocket.GetComponent<Rigidbody>();
            rocketRigidBody.isKinematic = false;
            rocketRigidBody.useGravity = true;

            var rocketRotation = rocket.transform.rotation.eulerAngles.y;

            var velocity = new Vector3(Mathf.Sin(rocketRotation * Mathf.Deg2Rad), 0f, Mathf.Cos(rocketRotation * Mathf.Deg2Rad));
            velocity *= 10f;
            velocity.y = 20f;

            rocketRigidBody.velocity = Vector3.zero;
            rocketRigidBody.angularVelocity = Vector3.zero;

            rocketRigidBody.AddForce(velocity, ForceMode.VelocityChange);
            rocketRigidBody.AddRelativeTorque(new Vector3(Random.Range(-40f, 40f), Random.Range(-40f, 40f), Random.Range(-40f, 40f)) * 100f);

            State = LLState.Impacted;
        }

        private void moveTweenComplete()
        {
            if (currentCharacterWayPoint < characterWayPoints.Count - 1)
            {
                currentCharacterWayPoint++;
                //reached the end:
                if (currentCharacterWayPoint == characterWayPoints.Count - 1)
                {

                    transform.parent.Find("MazeLetter").GetComponent<MazeLetter>().isDrawing = false;

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
                    MoveTween();

                //enable collider when we reach the second waypoint
                //if (currentCharacterWayPoint == 1)
                //myCollider.SetActive(true);
            }
        }
        public void initMovement()
        {
            if (characterIsMoving)
            {
                return;
            }

            transform.DOKill(false);
            characterIsMoving = true;
            GetComponent<Collider>().enabled = true;

            foreach (GameObject particle in particles)
            {
                particle.SetActive(true);
            }

            myCollider.enabled = false;

            // Test with tweens:
            MoveTween();
        }

        public void calculateMovementAndRotation()
        {
            //if(victory) return;

            Vector3 previousPosition = targetPos;
            float distance = (0.1f) - Camera.main.transform.position.y;
            targetPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -distance);
            targetPos = Camera.main.ScreenToWorldPoint(targetPos);

            if (previousPosition != initialPosition && previousPosition != targetPos)
            {
                //MazeGameManager.Instance.DrawLine (previousPosition, targetPos, Color.red);
                MazeGameManager.instance.appendToLine(targetPos);
            }



            if (previousPosition != targetPos)
            {
                characterWayPoints.Add(targetPos + new Vector3(0, 1, 0));
                MazeGameManager.instance.drawingTool.transform.position = targetPos + new Vector3(0, 1, 0);
            }

            if ((_fruits[_fruits.Count - 1].transform.position - targetPos).sqrMagnitude < 0.1f)
            {

                toggleVisibility(true);
                initMovement();
                MazeGameManager.instance.timer.StopTimer();
            }

        }

        public void Appear()
        {
            toggleVisibility(true);
            isAppearing = true;

            List<Vector3> trajectoryPoints = new List<Vector3>();

            var finalPosition = Fruits[0].transform.GetChild(0).gameObject.transform.position;

            int numTrajectoryPoints = 7;
            float yDecrement = (finalPosition.y - transform.position.y) / (numTrajectoryPoints + 1);

            float[] trajectoryPointXAnchors = { -0.7f, 0f, 0.7f, 0f, -0.75f, -0.4f, 0f };
            float[] trajectoryPointZAnchors = { 0f, 0.8f, 0f, -0.8f, -0.5f, 0.7f, 0.85f, 1.2f };

            trajectoryPoints.Add(transform.position);

            for (int i = 0; i < numTrajectoryPoints; i++)
            {
                Vector3 trajectoryPoint = new Vector3();
                trajectoryPoint.y = transform.position.y + (i + 1) * yDecrement;

                var frustumHeight = GetFrustumHeightAtDistance(Camera.main.transform.position.y - trajectoryPoint.y);
                var frustumWidth = GetFrustumWidth(frustumHeight);

                trajectoryPoint.x = frustumWidth * 0.5f * trajectoryPointXAnchors[i];
                trajectoryPoint.z = frustumHeight * 0.5f * trajectoryPointZAnchors[i];

                trajectoryPoints.Add(trajectoryPoint);
            }

            trajectoryPoints.Add(finalPosition);

            transform.DOPath(trajectoryPoints.ToArray(), 3, PathType.CatmullRom, PathMode.Ignore).OnWaypointChange((int index) =>
            {
                if (index + 1 < trajectoryPoints.Count)
                {
                    var dir = transform.position - trajectoryPoints[index + 1];
                    var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);// * initialRotation;
                                                                                  // transform.DORotateQuaternion(targetRotation, 0.007f);
                }
            }).OnComplete(() =>
            {
                toggleVisibility(false);
                isAppearing = false;

                //transform.rotation = initialRotation;
                MazeGameManager.instance.showCurrentTutorial();
            });

        }

        public void Flee()
        {
            StartCoroutine(Flee_Coroutine());
        }

        private IEnumerator Flee_Coroutine()
        {
            yield return new WaitForSeconds(0.25f);

            isFleeing = true;

            List<Vector3> fleePathPoints = new List<Vector3>();

            var cameraPosition = Camera.main.transform.position;

            var frustumHeight = GetFrustumHeightAtDistance(FLEE_PATH_ENDPOINT_DISTANCE_FROM_CAMERA);
            var frustumWidth = GetFrustumWidth(frustumHeight);

            Vector3 endPoint = new Vector3(cameraPosition.x + (frustumWidth / 2) * FLEE_PATH_ENDPOINT_X_ANCHOR,
                                            cameraPosition.y - FLEE_PATH_ENDPOINT_DISTANCE_FROM_CAMERA,
                                                cameraPosition.z + (frustumHeight / 2) * FLEE_PATH_ENDPOINT_Z_ANCHOR);

            Vector3 midPoint = transform.position + endPoint;
            midPoint *= 0.5f;

            frustumHeight = GetFrustumHeightAtDistance(cameraPosition.y - midPoint.y);
            frustumWidth = GetFrustumWidth(frustumHeight);

            midPoint = new Vector3(cameraPosition.x + (frustumWidth / 2) * FLEE_PATH_MIDPOINT_X_ANCHOR,
                                            midPoint.y,
                                                cameraPosition.z + (frustumHeight / 2) * FLEE_PATH_MIDPOINT_Z_ANCHOR);


            fleePathPoints.Add(transform.position);
            fleePathPoints.Add(midPoint);
            fleePathPoints.Add(endPoint);

            LL.Initialize(MazeGameManager.instance.currentLL);

            transform.DOPath(fleePathPoints.ToArray(), FLEE_PATH_DURATION, PathType.CatmullRom, PathMode.Ignore).OnWaypointChange((int index) =>
            {
                if (index < fleePathPoints.Count - 1)
                {
                    var dir = transform.position - fleePathPoints[index + 1];

                    brakeRotation = GetCorrectedRotationOfRocket(dir);
                    brakeRotation.z += 20f;

                    transform.DORotate(brakeRotation, 0.33f);
                }

            }).OnComplete(() =>
            {
                //wait then show cracks:
                StartCoroutine(waitAndPerformCallback(3.5f, () =>
                {
                    MazeGameManager.instance.showAllCracks();
                    donotHandleBorderCollision = true;
                    characterIsMoving = false;
                    transform.DOKill(false);

                    rocket.GetComponent<SphereCollider>().enabled = true;

                    var rocketRigidBody = rocket.GetComponent<Rigidbody>();
                    rocketRigidBody.isKinematic = false;
                    rocketRigidBody.useGravity = true;

                    State = LLState.Ragdolling;

                    ragdoll.deleteOnRagdollHit = true;

                    MazeGameManager.instance.ColorCurrentLinesAsIncorrect();

                    var tickPosition = transform.position;
                    tickPosition.z -= 1f;
                    tickPosition.y -= 1.5f;
                    Tutorial.TutorialUI.MarkNo(tickPosition, Tutorial.TutorialUI.MarkSize.Big);

                },
                () =>
                {
                    MazeGameManager.instance.lostCurrentLetter();
                }));
            });
        }

        public void Celebrate(System.Action OnCelebrationOver)
        {
            List<Vector3> celebrationPathPoints = new List<Vector3>();

            var cameraPosition = Camera.main.transform.position;

            var frustumHeight = GetFrustumHeightAtDistance(CELEBRATION_PATH_ENDPOINT_DISTANCE_FROM_CAMERA);
            var frustumWidth = GetFrustumWidth(frustumHeight);

            Vector3 endPoint = new Vector3(cameraPosition.x + (frustumWidth / 2) * CELEBRATION_PATH_ENDPOINT_X_ANCHOR,
                                            cameraPosition.y - CELEBRATION_PATH_ENDPOINT_DISTANCE_FROM_CAMERA,
                                                cameraPosition.z + (frustumHeight / 2) * CELEBRATION_PATH_ENDPOINT_Z_ANCHOR);

            Vector3 offscreenPoint = new Vector3(endPoint.x, endPoint.y, endPoint.z);
            offscreenPoint.x = cameraPosition.x + ((frustumWidth / 2) + 2f) * Mathf.Sign(CELEBRATION_PATH_ENDPOINT_X_ANCHOR);

            Vector3 midPoint = transform.position + endPoint;
            midPoint *= 0.5f;

            frustumHeight = GetFrustumHeightAtDistance(cameraPosition.y - midPoint.y);
            frustumWidth = GetFrustumWidth(frustumHeight);

            midPoint = new Vector3(cameraPosition.x + (frustumWidth / 2) * CELEBRATION_PATH_MIDPOINT_X_ANCHOR,
                                            midPoint.y,
                                                cameraPosition.z + (frustumHeight / 2) * CELEBRATION_PATH_MIDPOINT_Z_ANCHOR);


            celebrationPathPoints.Add(transform.position);
            celebrationPathPoints.Add(midPoint);
            celebrationPathPoints.Add(endPoint);
            celebrationPathPoints.Add(offscreenPoint);

            LL.Initialize(MazeGameManager.instance.currentLL);

            transform.DOPath(celebrationPathPoints.ToArray(), CELEBRATION_PATH_DURATION, PathType.CatmullRom, PathMode.Ignore).OnWaypointChange((int index) =>
            {
                if (index < celebrationPathPoints.Count - 2)
                {
                    var dir = transform.position - celebrationPathPoints[index + 1];

                    brakeRotation = GetCorrectedRotationOfRocket(dir);

                    transform.DORotate(brakeRotation, 0.33f);
                }

                else if (index == celebrationPathPoints.Count - 2)
                {
                    transform.DOPause();

                    State = LLState.Braked;

                    winParticleVFX.SetActive(true);

                    var tickPosition = transform.position;
                    tickPosition.z -= 1.5f;
                    tickPosition.x -= 0.5f;

                    Tutorial.TutorialUI.MarkYes(tickPosition, Tutorial.TutorialUI.MarkSize.Big);

                    transform.DOMove(transform.position + new Vector3(-0.5f, 0.5f, -0.5f) * 0.33f, 0.75f).SetEase(Ease.InOutSine).SetLoops(3, LoopType.Yoyo).OnComplete(() =>
                  {
                      State = LLState.Normal;

                      transform.DOPlay();

                      var dir = transform.position - celebrationPathPoints[index + 1];

                      brakeRotation = GetCorrectedRotationOfRocket(dir);

                      transform.DORotate(brakeRotation, 0.33f);
                  });
                }

            }).OnComplete(() =>
            {
                toggleVisibility(false);
                gameObject.SetActive(false);
                OnCelebrationOver();
            });
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case LLState.Normal:
                    break;
                case LLState.Braked:
                    transform.rotation = Quaternion.Euler(brakeRotation);
                    break;
                case LLState.Impacted:
                    if (stateTime >= 0.33f)
                    {
                        State = LLState.Ragdolling;
                    }
                    break;
                default:
                    break;
            }

            stateTime += Time.fixedDeltaTime;
        }

        void moveTowards(Vector3 position, float speed = 10, bool useUpVector = true)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

            var dir = transform.position - position;
            var angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            targetRotation = Quaternion.AngleAxis(-angle, useUpVector ? Vector3.up : Vector3.forward);// * initialRotation;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5);
        }
    }
}