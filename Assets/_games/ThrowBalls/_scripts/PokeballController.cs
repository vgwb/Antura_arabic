using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.ThrowBalls
{
    public class PokeballController : MonoBehaviour
    {
        public static readonly Vector3 POKEBALL_POSITION = new Vector3(-1.14f, 3.8f, -20f);
        public const float VELOCITY_SQUARED_LAUNCH_THRESOLD = 16;
        public const int TOUCH_BUFFER_SIZE = 5;

        public static PokeballController instance;
        public Rigidbody rigidBody;

        private Vector3 touchOffset;
        private float cameraDistance;
        private List<Vector3> touchPositionsInPx;
        private List<float> touchSpeedsInPxPerSecs;

        private float stopWhenYIsEqualToMe = 0;
        private float yVelocity = 0;
        private float zVelocity = 0;
        private Vector3 launchPoint;

        Vector3 letterPos = new Vector3(0, 1.8f, 29.5f);

        #region Temporary mouse control variables

        private Vector3 flickDirection;
        private Vector3 flickOrigin;
        private Vector3 velocity;
        private Vector3 lastPosition;
        public List<Vector3> positions;
        public List<float> times;

        #endregion

        void Awake()
        {
            instance = this;

            touchPositionsInPx = new List<Vector3>();
            touchSpeedsInPxPerSecs = new List<float>();

            positions = new List<Vector3>();
            times = new List<float>();
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
            touchPositionsInPx.Clear();
            touchSpeedsInPxPerSecs.Clear();

            flickDirection = new Vector3(0, 0, 0);
            velocity = new Vector3(0, 0, 0);
            lastPosition = new Vector3(POKEBALL_POSITION.x, POKEBALL_POSITION.y, POKEBALL_POSITION.z);
            positions.Clear();
            times.Clear();
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
            pos.y += yVelocity * time + launchPoint.y - letterPos.y;

            pos.z = launchPoint.z + zVelocity * time;

            return pos;
        }

        void FixedUpdate()
        {
            if (!rigidBody.isKinematic)
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
            if (transform.position.y <= stopWhenYIsEqualToMe)
            {
                rigidBody.isKinematic = true;
            }

            else
            {
                /*if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    Vector3 touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cameraDistance));
                    Vector3 position;

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            touchOffset.x = 0;// transform.position.x - touchPosInWorldUnits.x;
                            touchOffset.y = 0;// transform.position.y - touchPosInWorldUnits.y;
                            break;

                        case TouchPhase.Moved:
                            position = transform.position;
                            position.x = touchPosInWorldUnits.x + touchOffset.x;
                            position.y = touchPosInWorldUnits.y + touchOffset.y;
                            position.z = transform.position.z;

                            transform.position = position;

                            Vector3 touchPosInPx = new Vector3(touch.position.x, touch.position.y, 0);
                            float touchSpeedInPxPerSecs = ((touch.deltaPosition.magnitude / Screen.dpi) / touch.deltaTime);

                            if (touchPositionsInPx.Count == TOUCH_BUFFER_SIZE)
                            {
                                for (int i = 0; i < touchPositionsInPx.Count - 1; i++)
                                {
                                    touchPositionsInPx[i] = touchPositionsInPx[i + 1];
                                }
                                
                                touchPositionsInPx[TOUCH_BUFFER_SIZE - 1] = touchPosInPx;

                                for (int i = 0; i < touchSpeedsInPxPerSecs.Count - 1; i++)
                                {
                                    touchSpeedsInPxPerSecs[i] = touchSpeedsInPxPerSecs[i + 1];
                                }

                                touchSpeedsInPxPerSecs[TOUCH_BUFFER_SIZE - 1] = touchSpeedInPxPerSecs;
                            }

                            else
                            {
                                touchPositionsInPx.Add(touchPosInPx);
                                touchSpeedsInPxPerSecs.Add(touchSpeedInPxPerSecs);
                            }

                            break;

                        case TouchPhase.Stationary:
                            break;

                        case TouchPhase.Ended:

                            if (touchSpeedsInPxPerSecs.Count < 1)
                            {
                                return;
                            }

                            Vector3 flickDirection = new Vector3(0, 0, 0);

                            for (int i = 1; i < touchPositionsInPx.Count; i++)
                            {
                                flickDirection += (touchPositionsInPx[i] - touchPositionsInPx[i - 1]).normalized;
                            }

                            flickDirection /= touchPositionsInPx.Count;
                            flickDirection.Normalize();

                            float magnitude = 0;

                            for (int i = 0; i < touchSpeedsInPxPerSecs.Count; i++)
                            {
                                magnitude += touchSpeedsInPxPerSecs[i];
                            }

                            magnitude /= touchSpeedsInPxPerSecs.Count;
                            magnitude *= 5;
                            magnitude = Mathf.Clamp(magnitude, 20, 110);

                            rigidBody.isKinematic = false;

                            rigidBody.AddForce(new Vector3(flickDirection.x * magnitude * 0.75f, flickDirection.y * magnitude * 0.75f, 0.75f * magnitude), ForceMode.VelocityChange);
                            rigidBody.AddTorque(new Vector3(flickDirection.x * magnitude * 0.75f, flickDirection.y * magnitude * 0.75f, 0.75f * magnitude), ForceMode.VelocityChange);

                            break;
                        case TouchPhase.Canceled:
                            break;
                        default:
                            break;
                    }
                }*/

                velocity = (transform.position - lastPosition) / Time.deltaTime;
                lastPosition = transform.position;
            }
        }

        #region Temporary mouse controls
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
            flickDirection.Normalize();
            Vector3 mousePosInPx = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            if (positions.Count == 5)
            {
                positions[0] = positions[1];
                positions[1] = positions[2];
                positions[2] = positions[3];
                positions[3] = positions[4];
                positions[4] = mousePosInPx;
                times[0] = times[1];
                times[1] = times[2];
                times[2] = times[3];
                times[3] = times[4];
                times[4] = Time.time;
            }
            else
            {
                positions.Add(mousePosInPx);
                times.Add(Time.time);
            }
            transform.position = position;
        }
        void OnMouseUp()
        {
            /*if (velocity.sqrMagnitude < VELOCITY_SQUARED_LAUNCH_THRESOLD)
            {
                Reset();
                return;
            }
            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            Vector3 position = transform.position;
            position.x = mousePosInWorldUnits.x + touchOffset.x;
            position.y = mousePosInWorldUnits.y + touchOffset.y;
            position.z = transform.position.z;
            //flickDirection += 2 * (position - transform.position);
            flickDirection = new Vector3(0, 0, 0);
            for (int i = 1; i < positions.Count; i++)
            {
                flickDirection += (positions[i] - positions[i - 1]).normalized;
            }
            flickDirection /= positions.Count;
            velocity = (positions[positions.Count - 1] - positions[0]) / Screen.dpi;
            velocity /= times[times.Count - 1] - times[0];
            velocity *= 3;
            flickDirection.Normalize();
            float magnitude = velocity.magnitude;
            magnitude = Mathf.Clamp(magnitude, 20, 100);
            rigidBody.isKinematic = false;
            int velocityZ = Mathf.RoundToInt(Mathf.Max(flickDirection.x * magnitude * 0.5f, flickDirection.y * magnitude * 0.5f));
            velocityZ = velocityZ / 20;
            velocityZ = velocityZ * 20;
            velocityZ = Mathf.Clamp(velocityZ, 40, 80);
            Debug.Log("Magnitude = " + magnitude);
            rigidBody.AddForce(new Vector3(flickDirection.x * magnitude * 0.7f, flickDirection.y * magnitude * 0.7f, 0.7f * magnitude), ForceMode.VelocityChange);
            rigidBody.AddTorque(new Vector3(flickDirection.x * velocity.x, flickDirection.y * velocity.y, 80), ForceMode.VelocityChange);*/

            

            float velocityY = 50f;
            float velocityZ = letterPos.z - transform.position.z;

            velocityZ *= Constants.GRAVITY.y;

            float factor = (-1 * velocityY) - Mathf.Sqrt(Mathf.Pow(velocityY, 2) - (2 * (transform.position.y - letterPos.y) * Constants.GRAVITY.y));
            factor = Mathf.Pow(factor, -1);

            velocityZ *= factor;

            rigidBody.isKinematic = false;
            rigidBody.AddForceAtPosition(new Vector3(0, velocityY, velocityZ), transform.position, ForceMode.Impulse);

            stopWhenYIsEqualToMe = letterPos.y;

            yVelocity = velocityY;
            zVelocity = velocityZ;

            launchPoint = transform.position;
        }

        #endregion
    }
}