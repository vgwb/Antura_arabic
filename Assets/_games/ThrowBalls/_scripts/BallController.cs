using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ThrowBalls
{
    public class BallController : MonoBehaviour
    {
        public static Vector3 INITIAL_BALL_POSITION = new Vector3(0, 5.25f, -20f);
        private readonly Vector3 REBOUND_DESTINATION = new Vector3(0, 18f, -30f);
        public const float BALL_RESPAWN_TIME = 3f;
        public const float INTERCEPTION_PAUSE_TIME = 0.33f;
        public const float INTERCEPTION_RISE_DELTA_Y = 3f;
        public const float INTERCEPTION_RISE_TIME = 0.2f;
        public const float REBOUND_TIME = 1f;
        public const float TIME_TO_IDLE = 6f;

        public static BallController instance;

        public Rigidbody rigidBody;

        private enum State
        {
            Anchored, Dragging, Launched, Intercepted, Rebounding, Idle
        }

        private State state;
        private float stateTime;

        private float cameraDistance;

        private float yzStretchRange = 3f;

        void Awake()
        {
            instance = this;

            rigidBody.maxAngularVelocity = 100;
        }

        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            cameraDistance = 26;
            INITIAL_BALL_POSITION.y = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 3, cameraDistance)).y;

            Reset();
        }

        public void Reset()
        {
            transform.position = INITIAL_BALL_POSITION;

            rigidBody.isKinematic = true;
            rigidBody.angularVelocity = new Vector3(0, 0, 0);
            rigidBody.velocity = new Vector3(0, 0, 0);
            rigidBody.isKinematic = false;
            SetState(State.Anchored);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            BallShadowController.instance.Enable();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            BallShadowController.instance.Disable();
        }

        private void SetState(State state)
        {
            this.state = state;

            switch (state)
            {
                case State.Anchored:
                    rigidBody.isKinematic = true;
                    break;
                case State.Dragging:
                    rigidBody.isKinematic = true;
                    break;
                case State.Launched:
                    rigidBody.isKinematic = false;
                    break;
                case State.Intercepted:
                    rigidBody.isKinematic = true;
                    break;
                case State.Rebounding:
                    rigidBody.isKinematic = false;
                    break;
                case State.Idle:
                    rigidBody.isKinematic = true;
                    if (!ThrowBallsGameManager.Instance.IsTutorialLevel())
                    {
                        AnturaController.instance.Reset();
                        AnturaController.instance.EnterScene();
                    }

                    break;
                default:
                    break;
            }

            stateTime = 0;
        }

        public bool IsLaunched()
        {
            return state == State.Launched;
        }

        public void OnIntercepted(bool interceptedByLetter)
        {
            if (state != State.Intercepted)
            {
                SetState(State.Intercepted);

                if (interceptedByLetter)
                {
                    StartCoroutine(OnInterceptedCoroutine());
                }
            }
        }

        private IEnumerator OnInterceptedCoroutine()
        {
            //yield return new WaitForSeconds(INTERCEPTION_PAUSE_TIME);

            float destinationY = transform.position.y + INTERCEPTION_RISE_DELTA_Y;
            float yIncrement = (Time.fixedDeltaTime * INTERCEPTION_RISE_DELTA_Y) / INTERCEPTION_RISE_TIME;

            float riseTime = 0;

            while (riseTime < INTERCEPTION_RISE_TIME)
            {
                Vector3 position = transform.position;
                position.y += yIncrement;
                transform.position = position;
                riseTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(INTERCEPTION_PAUSE_TIME);

            SetState(State.Rebounding);

            Vector3 initialVelocity = new Vector3();
            initialVelocity.x = (REBOUND_DESTINATION.x - transform.position.x) / REBOUND_TIME;
            initialVelocity.z = (REBOUND_DESTINATION.z - transform.position.z) / REBOUND_TIME;

            initialVelocity.y = (REBOUND_DESTINATION.y - (Constants.GRAVITY.y * Mathf.Pow(REBOUND_TIME, 2f) * 0.5f) - transform.position.y) / REBOUND_TIME;

            rigidBody.AddForce(initialVelocity, ForceMode.VelocityChange);
        }

        void FixedUpdate()
        {
            if (state == State.Launched)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                if (transform.position.y < -9 || stateTime > BALL_RESPAWN_TIME)
                {
                    ThrowBallsGameManager.Instance.OnBallLost();
                    Reset();
                }
            }

            else if (state == State.Rebounding)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                if ((transform.position - REBOUND_DESTINATION).sqrMagnitude <= 1)
                {
                    UIController.instance.OnScreenCracked();
                    ThrowBallsGameManager.Instance.OnBallLost();
                    Reset();
                }
            }

            else if (state == State.Anchored)
            {
                if (stateTime >= TIME_TO_IDLE)
                {
                    SetState(State.Idle);
                }
            }
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);

                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.gameObject.tag == Constants.TAG_POKEBALL
                            && (state == State.Anchored || state == State.Idle))
                        {
                            SetState(State.Dragging);
                        }

                        break;
                    case TouchPhase.Moved:
                        if (state == State.Dragging)
                        {
                            float clampedInputY = touch.position.y > Screen.height / 3 ? Screen.height / 3 : touch.position.y;

                            Vector3 touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, clampedInputY, cameraDistance));

                            float yzFactor = 1 - (clampedInputY / Screen.height) * 3;
                            touchPosInWorldUnits.y = INITIAL_BALL_POSITION.y - yzFactor * yzStretchRange;
                            touchPosInWorldUnits.z = INITIAL_BALL_POSITION.z - yzFactor * yzStretchRange;

                            transform.position = touchPosInWorldUnits;
                        }

                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        if (state == State.Dragging)
                        {
                            Vector3 forceToApply = SlingshotController.instance.GetLaunchForce();
                            rigidBody.isKinematic = false;
                            rigidBody.AddForce(forceToApply, ForceMode.VelocityChange);
                            SetState(State.Launched);
                        }

                        break;
                    case TouchPhase.Canceled:
                        break;
                    default:
                        break;
                }
            }

            stateTime += Time.deltaTime;
        }

        #region Mouse controls
        void OnMouseDown()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || !(state == State.Idle || state == State.Anchored))
            {
                return;
            }

            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));

            SetState(State.Dragging);
        }
        void OnMouseDrag()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            float clampedInputY = Input.mousePosition.y > Screen.height / 3 ? Screen.height / 3 : Input.mousePosition.y;

            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, clampedInputY, cameraDistance));

            float yzFactor = 1 - (clampedInputY / Screen.height) * 3;
            mousePosInWorldUnits.y = INITIAL_BALL_POSITION.y - yzFactor * yzStretchRange;
            mousePosInWorldUnits.z = INITIAL_BALL_POSITION.z - yzFactor * yzStretchRange;

            transform.position = mousePosInWorldUnits;
        }

        void OnMouseUp()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            Vector3 forceToApply = SlingshotController.instance.GetLaunchForce();
            rigidBody.isKinematic = false;
            rigidBody.AddForce(forceToApply, ForceMode.VelocityChange);
            SetState(State.Launched);
        }

        #endregion

    }
}