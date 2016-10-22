using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ThrowBalls
{
    public class PokeballController : MonoBehaviour
    {
        public static readonly Vector3 POKEBALL_POSITION = new Vector3(0, 5.25f, -20f);

        public static PokeballController instance;

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

            Reset();
        }

        public void Reset()
        {
            transform.position = POKEBALL_POSITION;

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
        }

        public void Disable()
        {
            gameObject.SetActive(false);
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

                            float yzFactor = 1 - (touch.position.y / Screen.height) * 3;
                            touchPosInWorldUnits.y = POKEBALL_POSITION.y - yzFactor * yzStretchRange;
                            touchPosInWorldUnits.z = POKEBALL_POSITION.z - yzFactor * yzStretchRange;

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

            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));

            float yzFactor = 1 - (Input.mousePosition.y / Screen.height) * 3;
            mousePosInWorldUnits.y = POKEBALL_POSITION.y - yzFactor * yzStretchRange;
            mousePosInWorldUnits.z = POKEBALL_POSITION.z - yzFactor * yzStretchRange;

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