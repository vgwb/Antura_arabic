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

        void Awake()
        {
            instance = this;

            touchPositionsInPx = new List<Vector3>();
            touchSpeedsInPxPerSecs = new List<float>();
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
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

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
                }
            }
        }
    }
}