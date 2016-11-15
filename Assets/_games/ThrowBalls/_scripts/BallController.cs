using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ThrowBalls
{
    public class BallController : MonoBehaviour
    {
        public static Vector3 INITIAL_BALL_POSITION = new Vector3(0, 5.25f, -20f);
        public static BallController instance;

        public Rigidbody rigidBody;
        public bool IsLaunched
        {
            get
            {
                return isLaunched;
            }
        }

        private bool isLaunched;
        private bool isHeld;
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

            cameraDistance = 23;
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
            isLaunched = false;

            isHeld = false;
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

        void FixedUpdate()
        {
            if (isLaunched)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                if (transform.position.y < -9)
                {
                    ThrowBallsGameManager.Instance.OnBallLost();
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
            isLaunched = true;
        }

        #endregion

    }
}