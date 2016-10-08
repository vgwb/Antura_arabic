using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class PokeballController : MonoBehaviour
    {
        public static readonly Vector3 POKEBALL_POSITION = new Vector3(-1.14f, 3.8f, -20f);
        public const float VELOCITY_SQUARED_LAUNCH_THRESOLD = 16;

        public static PokeballController instance;

        private Vector3 touchOffset;
        private float cameraDistance;
        private Vector3 flickDirection;
        private Vector3 flickOrigin;
        private Vector3 velocity;
        private Vector3 lastPosition;
        public Rigidbody rigidBody;

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            Reset();
        }

        void OnMouseDown()
        {
            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            touchOffset.x = transform.position.x - mousePosInWorldUnits.x;
            touchOffset.y = transform.position.y - mousePosInWorldUnits.y;
        }

        void OnMouseDrag()
        {
            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            Vector3 position = transform.position;
            position.x = mousePosInWorldUnits.x + touchOffset.x;
            position.y = mousePosInWorldUnits.y + touchOffset.y;
            position.z = transform.position.z;

            //flickDirection += position - transform.position;

            flickDirection = position - transform.position;

            transform.position = position;
        }

        void OnMouseUp()
        {
            if (velocity.sqrMagnitude < VELOCITY_SQUARED_LAUNCH_THRESOLD)
            {
                Reset();
                return;
            }

            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            Vector3 position = transform.position;
            position.x = mousePosInWorldUnits.x + touchOffset.x;
            position.y = mousePosInWorldUnits.y + touchOffset.y;
            position.z = transform.position.z;

            flickDirection += 2 * (position - transform.position);

            flickDirection.Normalize();
            rigidBody.isKinematic = false;
            rigidBody.AddForce(new Vector3(flickDirection.x * 60, flickDirection.y * 60, 60), ForceMode.VelocityChange);
            rigidBody.AddTorque(new Vector3(flickDirection.x * 60, flickDirection.y * 60, 60), ForceMode.VelocityChange);
        }

        void Update()
        {
            if (!rigidBody.isKinematic)
            {
                rigidBody.AddForce(24 * Physics.gravity, ForceMode.Acceleration);

                if (transform.position.y < -9)
                {
                    ThrowBallsGameManager.Instance.OnPokeballLost();
                    Reset();
                }
            }

            else
            {
                velocity = (transform.position - lastPosition) / Time.deltaTime;
                lastPosition = transform.position;
            }
        }

        public void Reset()
        {
            transform.position = POKEBALL_POSITION;
            rigidBody.isKinematic = true;
            flickDirection = new Vector3(0, 0, 0);
            velocity = new Vector3(0, 0, 0);
            lastPosition = new Vector3(POKEBALL_POSITION.x, POKEBALL_POSITION.y, POKEBALL_POSITION.z);
            Enable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }
    }
}