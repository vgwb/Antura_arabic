using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ThrowBalls
{
    public class BallController : MonoBehaviour
    {
        public static Vector3 INITIAL_BALL_POSITION = new Vector3(0, 5.25f, -20f);
        private readonly Vector3 REBOUND_DESTINATION = new Vector3(0, 15f, -30f);
        public const float BALL_RESPAWN_TIME = 3f;
        public const float REBOUND_TIME = 0.2f;
        public const float SCREEN_HANG_TIME = 0.5f;
        public const float DROP_TIME = 0.5f;
        public const float TIME_TO_IDLE = 6f;

        public static BallController instance;

        public Rigidbody rigidBody;
        private SphereCollider sphereCollider;

        private TrailRenderer trailRenderer;

        private IAudioManager audioManager;

        private enum State
        {
            Chased, Anchored, Dragging, Launched, Intercepted, Rebounding, Hanging, Dropping, Idle
        }

        private State state;
        private float stateTime;

        private float cameraDistance;

        private float yzStretchRange = 3f;

        void Awake()
        {
            instance = this;

            rigidBody.maxAngularVelocity = 100;

            sphereCollider = GetComponent<SphereCollider>();

            trailRenderer = GetComponent<TrailRenderer>();

            audioManager = ThrowBallsConfiguration.Instance.Context.GetAudioManager();
        }

        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            cameraDistance = 26;
            INITIAL_BALL_POSITION.y = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 3, cameraDistance)).y;

            IInputManager inputManager = ThrowBallsConfiguration.Instance.Context.GetInputManager();
            inputManager.onPointerDown += OnPointerDown;
            inputManager.onPointerDrag += OnPointerDrag;
            inputManager.onPointerUp += OnPointerUp;

            //Reset();

            SetState(State.Chased);

            ArrowBodyController.instance.Disable();
            ArrowHeadController.instance.Disable();
        }

        private void OnPointerDown()
        {
            if (!IsLaunched())
            {
                SetState(State.Dragging);
            }
        }

        private void OnPointerDrag()
        {
            if (state == State.Dragging)
            {
                Vector2 lastTouch = ThrowBallsConfiguration.Instance.Context.GetInputManager().LastPointerPosition;

                float clampedInputY = lastTouch.y > Screen.height / 3 ? Screen.height / 3 : lastTouch.y;

                Vector3 touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(lastTouch.x, clampedInputY, cameraDistance));

                float yzFactor = 1 - (clampedInputY / Screen.height) * 3;
                touchPosInWorldUnits.y = INITIAL_BALL_POSITION.y - yzFactor * yzStretchRange;
                touchPosInWorldUnits.z = INITIAL_BALL_POSITION.z - yzFactor * yzStretchRange;

                transform.position = touchPosInWorldUnits; 
            }
        }

        private void OnPointerUp()
        {
            if (state == State.Dragging)
            {
                Vector3 forceToApply = SlingshotController.instance.GetLaunchForce();
                rigidBody.isKinematic = false;
                rigidBody.AddForce(forceToApply, ForceMode.VelocityChange);
                SetState(State.Launched);

                ArrowBodyController.instance.Disable();
                ArrowHeadController.instance.Disable();
            }
        }

        public void OnCollisionEnter()
        {
            if (state != State.Chased)
            {
                audioManager.PlaySound(Sfx.BallHit);
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Bush")
            {
                stateTime += 2f;
            }
        }

        public void Reset()
        {
            transform.position = INITIAL_BALL_POSITION;

            rigidBody.isKinematic = true;
            rigidBody.angularVelocity = new Vector3(0, 0, 0);
            rigidBody.velocity = new Vector3(0, 0, 0);
            rigidBody.isKinematic = false;
            rigidBody.useGravity = false;
            sphereCollider.enabled = true;
            SetState(State.Anchored);

            ArrowBodyController.instance.Reset();
            ArrowHeadController.instance.Reset();

            ArrowBodyController.instance.Enable();
            ArrowHeadController.instance.Enable();

            trailRenderer.Clear();
            trailRenderer.enabled = false;
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

            if (state != State.Idle || state != State.Chased)
            {
                Physics.IgnoreLayerCollision(10, 12);
            }

            switch (state)
            {
                case State.Anchored:
                    rigidBody.isKinematic = false;
                    sphereCollider.enabled = true;
                    break;
                case State.Dragging:
                    rigidBody.isKinematic = false;
                    break;
                case State.Launched:
                    rigidBody.isKinematic = false;
                    trailRenderer.enabled = true;

                    audioManager.PlaySound(Sfx.ThrowObj);

                    break;
                case State.Intercepted:
                    rigidBody.isKinematic = true;
                    sphereCollider.enabled = false;
                    break;
                case State.Rebounding:
                    rigidBody.isKinematic = false;
                    sphereCollider.enabled = false;
                    break;
                case State.Hanging:
                    rigidBody.isKinematic = true;
                    break;
                case State.Dropping:
                    rigidBody.isKinematic = false;
                    break;
                case State.Idle:
                    rigidBody.isKinematic = false;

                    if (!GameState.instance.IsTutorialLevel())
                    {
                        AnturaController.instance.Enable();
                        AnturaController.instance.Reset();
                        AnturaController.instance.EnterScene();
                    }

                    Physics.IgnoreLayerCollision(10, 12, false);

                    break;
                case State.Chased:
                    trailRenderer.enabled = false;
                    Physics.IgnoreLayerCollision(10, 12, false);
                    break;
                default:
                    break;
            }

            stateTime = 0;
        }

        public bool IsLaunched()
        {
            return !(state == State.Anchored || state == State.Dragging || state == State.Idle);
        }

        public bool IsIdle()
        {
            return state == State.Idle;
        }

        public void OnIntercepted()
        {
            if (state != State.Intercepted)
            {
                SetState(State.Intercepted);
            }
        }

        public void OnRebounded()
        {
            if (state != State.Rebounding)
            {
                SetState(State.Rebounding);

                Vector3 initialVelocity = new Vector3();
                initialVelocity.x = (REBOUND_DESTINATION.x - transform.position.x) / REBOUND_TIME;
                initialVelocity.y = (REBOUND_DESTINATION.y - transform.position.y) / REBOUND_TIME;
                initialVelocity.z = (REBOUND_DESTINATION.z - transform.position.z) / REBOUND_TIME;

                rigidBody.velocity = new Vector3(0, 0, 0);
                rigidBody.AddForce(initialVelocity, ForceMode.VelocityChange);
            }
        }

        void FixedUpdate()
        {
            if (state == State.Launched)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                if (transform.position.y < -9 || stateTime > BALL_RESPAWN_TIME)
                {
                    GameState.instance.OnBallLost();
                    Reset();
                }
            }

            else if (state == State.Rebounding)
            {
                if (transform.position.z + rigidBody.velocity.z * Time.fixedDeltaTime <= REBOUND_DESTINATION.z)
                {
                    transform.position = REBOUND_DESTINATION;
                    UIController.instance.OnScreenCracked();
                    SetState(State.Hanging);
                }
            }

            else if (state == State.Anchored)
            {
                if (stateTime >= TIME_TO_IDLE)
                {
                    SetState(State.Idle);
                }
            }

            else if (state == State.Hanging)
            {
                if (stateTime >= SCREEN_HANG_TIME)
                {
                    SetState(State.Dropping);
                }
            }

            else if (state == State.Dropping)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                if (stateTime >= DROP_TIME)
                {
                    GameState.instance.OnBallLost();
                    Reset();
                }
            }
        }

        void Update()
        {
            stateTime += Time.deltaTime;
        }
    }
}