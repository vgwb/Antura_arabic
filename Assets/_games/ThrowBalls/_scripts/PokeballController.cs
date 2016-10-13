using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public List<Vector3> positions;
        public List<float> times;
        public List<float> speeds;

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            positions = new List<Vector3>();
            times = new List<float>();
            speeds = new List<float>();
            Reset();
        }

        /*void OnMouseDown()
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
                times[4] = Touch;
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
            rigidBody.AddTorque(new Vector3(flickDirection.x * velocity.x, flickDirection.y * velocity.y, 80), ForceMode.VelocityChange);
        }*/

        void Update()
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

            else
            {


                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Vector3 touchPosInWorldUnits;
                    Vector3 position;

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cameraDistance));
                            touchOffset.x = 0;// transform.position.x - touchPosInWorldUnits.x;
                            touchOffset.y = 0;// transform.position.y - touchPosInWorldUnits.y;
                            break;
                        case TouchPhase.Moved:
                            touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cameraDistance));
                            position = transform.position;
                            position.x = touchPosInWorldUnits.x + touchOffset.x;
                            position.y = touchPosInWorldUnits.y + touchOffset.y;
                            position.z = transform.position.z;

                            //flickDirection += position - transform.position;

                            flickDirection = position - transform.position;
                            flickDirection.Normalize();

                            Vector3 mousePosInPx = new Vector3(touch.position.x, touch.position.y, 0);

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
                                times[4] = touch.deltaTime;

                                speeds[0] = speeds[1];
                                speeds[1] = speeds[2];
                                speeds[2] = speeds[3];
                                speeds[3] = speeds[4];
                                speeds[4] = ((touch.deltaPosition.magnitude / Screen.dpi) / touch.deltaTime);
                            }

                            else
                            {
                                positions.Add(mousePosInPx);
                                times.Add(touch.deltaTime);
                                speeds.Add((touch.deltaPosition.magnitude / Screen.dpi) / touch.deltaTime);
                            }

                            transform.position = position;
                            break;
                        case TouchPhase.Stationary:
                            break;
                        case TouchPhase.Ended:
                            /*if (velocity.sqrMagnitude < VELOCITY_SQUARED_LAUNCH_THRESOLD)
                            {
                                Reset();
                                return;
                            }*/

                            if (speeds.Count < 1)
                            {
                                return;
                            }

                            touchPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
                            position = transform.position;
                            position.x = touchPosInWorldUnits.x + touchOffset.x;
                            position.y = touchPosInWorldUnits.y + touchOffset.y;
                            position.z = transform.position.z;

                            //flickDirection += 2 * (position - transform.position);

                            flickDirection = new Vector3(0, 0, 0);

                            for (int i = 1; i < positions.Count; i++)
                            {
                                flickDirection += (positions[i] - positions[i - 1]).normalized;
                            }

                            float aveTime = 0;

                            for (int i = 0; i < times.Count; i++)
                            {
                                aveTime += times[i];
                            }

                            flickDirection /= positions.Count;

                            velocity = (positions[positions.Count - 1] - positions[0]) / Screen.dpi;
                            velocity /= aveTime;

                            velocity *= 3;

                            flickDirection.Normalize();

                            

                            


                            float magnitude = velocity.magnitude;
                            magnitude = 0;

                            for (int i = 0; i < speeds.Count; i++)
                            {
                                magnitude += speeds[i];
                            }

                            magnitude /= speeds.Count;

                            magnitude *= 5;

                            magnitude = Mathf.Clamp(magnitude, 20, 110);

                            rigidBody.isKinematic = false;

                            int velocityZ = Mathf.RoundToInt(Mathf.Max(flickDirection.x * magnitude * 0.5f, flickDirection.y * magnitude * 0.5f));
                            velocityZ = velocityZ / 20;
                            velocityZ = velocityZ * 20;
                            velocityZ = Mathf.Clamp(velocityZ, 40, 80);

                            Debug.Log("Magnitude = " + magnitude);

                            rigidBody.AddForce(new Vector3(flickDirection.x * magnitude * 0.75f, flickDirection.y * magnitude * 0.75f, 0.75f * magnitude), ForceMode.VelocityChange);
                            rigidBody.AddTorque(new Vector3(flickDirection.x * velocity.x, flickDirection.y * velocity.y, 80), ForceMode.VelocityChange);
                            break;
                        case TouchPhase.Canceled:
                            break;
                        default:
                            break;
                    }
                    /*Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
                    touchOffset.x = transform.position.x - mousePosInWorldUnits.x;
                    touchOffset.y = transform.position.y - mousePosInWorldUnits.y;*/
                }



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
            positions.Clear();
            times.Clear();
            speeds.Clear();
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