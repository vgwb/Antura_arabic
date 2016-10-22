using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ThrowBalls
{
    public class PokeballController : MonoBehaviour
    {
        public static readonly Vector3 POKEBALL_POSITION = new Vector3(-1.14f, 3.8f, -20f);
        public const float VELOCITY_SQUARED_LAUNCH_THRESOLD = 16;
        public const int TOUCH_BUFFER_SIZE = 6;
        private readonly Vector3 POKEBALL_LETTER_CENTER_OFFSET = new Vector3(0, 3.3f, 0);

        public static PokeballController instance;
        public Rigidbody rigidBody;
        public GameObject ringEffectPrefab;
        public TrailRendererController greenTrailRenderer;
        public TrailRendererController yellowTrailRenderer;
        public TrailRendererController redTrailRenderer;

        public bool IsLaunched
        {
            get
            {
                return isLaunched;
            }
        }

        public enum ChargeStrength
        {
            None, Low, Medium, High
        };
        private ChargeStrength chargeStrength;
        private float cameraDistance;
        private List<Vector3> touchPositionsInPx;
        private List<float> touchSpeedsInInchesPerSecs;
        private Vector3 touchOrigin;

        private float yVelocity = 0;
        private float zVelocity = 0;
        private Vector3 launchPoint;
        private bool isLaunched;
        private Vector3 target;
        private float timeOfLaunch;

        private bool isHeld;

        #region Temporary mouse control variables

        private Vector3 flickDirection;
        private Vector3 flickOrigin;
        private Vector3 velocity;
        private Vector3 lastPosition;
        public List<Vector3> positions;
        public List<float> times;
        public AudioSource audioSource;
        public AudioClip pokeballLowClip;
        public AudioClip pokeballMedClip;
        public AudioClip pokeballHighClip;

        #endregion

        void Awake()
        {
            instance = this;

            touchPositionsInPx = new List<Vector3>();
            touchSpeedsInInchesPerSecs = new List<float>();

            positions = new List<Vector3>();
            times = new List<float>();

            rigidBody.maxAngularVelocity = 100;
        }

        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            Reset();
        }
        
        public void Reset()
        {
            transform.position = POKEBALL_POSITION;
            touchPositionsInPx.Clear();
            touchSpeedsInInchesPerSecs.Clear();

            flickDirection = new Vector3(0, 0, 0);
            velocity = new Vector3(0, 0, 0);
            lastPosition = new Vector3(POKEBALL_POSITION.x, POKEBALL_POSITION.y, POKEBALL_POSITION.z);
            positions.Clear();
            times.Clear();

            rigidBody.isKinematic = true;
            rigidBody.angularVelocity = new Vector3(0, 0, 0);
            rigidBody.velocity = new Vector3(0, 0, 0);
            rigidBody.isKinematic = false;
            isLaunched = false;

            chargeStrength = ChargeStrength.None;
            ParticleSystemController.instance.OnPositionUpdate(POKEBALL_POSITION);
            ParticleSystemController.instance.OnChargeStrengthUpdate(chargeStrength);

            greenTrailRenderer.SetIsFollowPokeball(false);
            yellowTrailRenderer.SetIsFollowPokeball(false);
            redTrailRenderer.SetIsFollowPokeball(false);

            isHeld = false;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public Vector3 ComputePosition(float time)
        {
            Vector3 pos = new Vector3();
            pos.x = transform.position.x;

            pos.y = Constants.GRAVITY.y * Mathf.Pow(time, 2);
            pos.y /= 2;
            pos.y += yVelocity * time + launchPoint.y - target.y;

            pos.z = launchPoint.z + zVelocity * time;

            return pos;
        }

        void FixedUpdate()
        {
            if (isLaunched)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                if (transform.position.y < -9)
                {
                    ThrowBallsGameManager.Instance.OnPokeballLost();
                    Reset();
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

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.gameObject.tag == Constants.TAG_POKEBALL && !isLaunched)
                        {
                            isHeld = true;
                        }

                        break;
                    case TouchPhase.Moved:
                        if (isHeld)
                        {
                            Vector3 touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cameraDistance));
                            transform.position = touchPosInWorldUnits;
                        }

                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        if (isHeld)
                        {
                            Vector3 forceToApply = SlingshotController.instance.GetLaunchForce();
                            rigidBody.isKinematic = false;
                            rigidBody.AddForce(forceToApply, ForceMode.VelocityChange);
                            isLaunched = true;
                        }

                        break;
                    case TouchPhase.Canceled:
                        break;
                    default:
                        break;
                }
            }
        }

        #region Mouse controls
        void OnMouseDown()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            touchOrigin = mousePosInWorldUnits;
        }
        void OnMouseDrag()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
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
            isLaunched = true;
        }

        #endregion

    }
}