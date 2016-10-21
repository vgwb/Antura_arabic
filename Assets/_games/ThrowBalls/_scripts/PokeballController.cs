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

        public enum ChargeStrength
        {
            None, Low, Medium, High
        };
        private ChargeStrength chargeStrength;
        private Vector3 touchOffset;
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

            touchOffset.x = -1;
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
                Vector3 targetGravity = (target - transform.position).normalized * Constants.GRAVITY.magnitude * SROptions.Current.TargetGravityFactor;
                targetGravity.y = 0;
                targetGravity.z = 0;
                rigidBody.AddForce(targetGravity, ForceMode.Acceleration);

                if (transform.position.y < -9)
                {
                    ThrowBallsGameManager.Instance.OnPokeballLost();
                    Reset();
                }

                
            }
        }

        void Update()
        {
            ParticleSystemController.instance.OnPositionUpdate(transform.position);

            if (isLaunched)
            {
                //transform.position = ComputePosition(Time.time - timeOfLaunch);
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
                            RaycastHit hit;
                            if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, Mathf.Infinity) && hit.collider.gameObject.tag == Constants.TAG_POKEBALL)
                            {
                                touchOffset.x = 0;
                                touchOffset.y = 0;
                                touchOrigin = new Vector3(touch.position.x, touch.position.y, 0);
                            }
                            
                            break;

                        case TouchPhase.Moved:
                            if (touchOffset.x < 0)
                            {
                                return;
                            }
                            position = transform.position;
                            position.x = touchPosInWorldUnits.x + touchOffset.x;
                            position.y = touchPosInWorldUnits.y + touchOffset.y;
                            position.z = transform.position.z;

                            transform.position = position;

                            Vector3 touchPosInPx = new Vector3(touch.position.x, touch.position.y, 0);


                            float deltaYFromOrigin = (touch.position.y - touchOrigin.y) / 420;

                            if (deltaYFromOrigin < SROptions.Current.TouchRecordingYThreshold)
                            {
                                return;
                            }




                            float touchSpeedInPxPerSecs = ((touch.deltaPosition.magnitude / (Screen.dpi * 0.33f)) / touch.deltaTime);
                            //float touchSpeedInPxPerSecs = ((touch.deltaPosition.magnitude / 138f) / touch.deltaTime);





                            if (touchPositionsInPx.Count == TOUCH_BUFFER_SIZE)
                            {
                                for (int i = 0; i < touchPositionsInPx.Count - 1; i++)
                                {
                                    touchPositionsInPx[i] = touchPositionsInPx[i + 1];
                                }

                                touchPositionsInPx[TOUCH_BUFFER_SIZE - 1] = touchPosInPx;

                                for (int i = 0; i < touchSpeedsInInchesPerSecs.Count - 1; i++)
                                {
                                    touchSpeedsInInchesPerSecs[i] = touchSpeedsInInchesPerSecs[i + 1];
                                }

                                touchSpeedsInInchesPerSecs[TOUCH_BUFFER_SIZE - 1] = touchSpeedInPxPerSecs;
                            }

                            else
                            {
                                touchPositionsInPx.Add(touchPosInPx);
                                touchSpeedsInInchesPerSecs.Add(touchSpeedInPxPerSecs);
                            }

                            float unitYRotation = Mathf.PI * 2;

                            flickDirection = new Vector3(0, 0, 0);
                            for (int i = 1; i < touchPositionsInPx.Count; i++)
                            {
                                flickDirection += (touchPositionsInPx[i] - touchPositionsInPx[i - 1]).normalized;
                            }
                            //flickDirection /= touchPositionsInPx.Count;
                            flickDirection.Normalize();

                            float magnitudee = 0;

                            for (int i = 0; i < touchSpeedsInInchesPerSecs.Count; i++)
                            {
                                magnitudee += touchSpeedsInInchesPerSecs[i];
                            }

                            magnitudee /= touchSpeedsInInchesPerSecs.Count;


                            Debug.Log("Delta Y in inches = " + deltaYFromOrigin);
                            Debug.Log("Velocity = " + magnitudee);

                            if ((magnitudee > SROptions.Current.HighStrengthThreshold && deltaYFromOrigin > SROptions.Current.HighStrengthYDelta) || magnitudee > SROptions.Current.HighStrengthBruteForce)
                            {
                                SetChargeStrength(ChargeStrength.High);
                                rigidBody.angularVelocity = new Vector3(unitYRotation * 6, 0, 0);
                            }

                            else if ((magnitudee > SROptions.Current.MedStrengthThreshold && deltaYFromOrigin > SROptions.Current.MedStrengthYDelta) && chargeStrength < ChargeStrength.High)
                            {
                                SetChargeStrength(ChargeStrength.Medium);
                                rigidBody.angularVelocity = new Vector3(unitYRotation * 3.25f, 0, 0);
                            }

                            else if ((magnitudee > SROptions.Current.LowStrengthThreshold && deltaYFromOrigin > SROptions.Current.LowStrengthYDelta) && chargeStrength < ChargeStrength.Medium)
                            {
                                SetChargeStrength(ChargeStrength.Low);
                                rigidBody.angularVelocity = new Vector3(unitYRotation * 1.5f, 0, 0);
                            }

                            else if (chargeStrength < ChargeStrength.Low)
                            {
                                SetChargeStrength(ChargeStrength.None);
                                rigidBody.angularVelocity = new Vector3(0, 0, 0);
                            }

                            break;

                        case TouchPhase.Stationary:
                            break;

                        case TouchPhase.Ended:


                            if (touchSpeedsInInchesPerSecs.Count < 1 || touchOffset.x < 0)
                            {
                                return;
                            }


                            flickDirection = new Vector3(0, 0, 0);
                            for (int i = 1; i < touchPositionsInPx.Count; i++)
                            {
                                flickDirection += (touchPositionsInPx[i] - touchPositionsInPx[i - 1]).normalized;
                            }
                            flickDirection.Normalize();





                            float magnitude = 0;

                            for (int i = 0; i < touchSpeedsInInchesPerSecs.Count; i++)
                            {
                                magnitude += touchSpeedsInInchesPerSecs[i];
                            }

                            magnitude /= touchSpeedsInInchesPerSecs.Count;


                            Debug.Log("Delta Y in inches = " + ((touch.position.y - touchOrigin.y) / 420));
                            Debug.Log("Velocity = " + magnitude);


                            Vector3 letterPos = new Vector3();
                            float velocityY = 0f;

                            greenTrailRenderer.Reset();
                            yellowTrailRenderer.Reset();
                            redTrailRenderer.Reset();

                            switch (chargeStrength)
                            {
                                case ChargeStrength.None:
                                    Reset();
                                    return;
                                    break;
                                case ChargeStrength.Low:
                                    letterPos = ThrowBallsGameManager.Instance.GetPositionOfLetter(0);
                                    velocityY = SROptions.Current.YVelocity_Low;
                                    greenTrailRenderer.SetIsFollowPokeball(true);
                                    break;
                                case ChargeStrength.Medium:
                                    letterPos = ThrowBallsGameManager.Instance.GetPositionOfLetter(1);
                                    velocityY = SROptions.Current.YVelocity_Med;
                                    yellowTrailRenderer.SetIsFollowPokeball(true);
                                    break;
                                case ChargeStrength.High:
                                    letterPos = ThrowBallsGameManager.Instance.GetPositionOfLetter(2);
                                    velocityY = SROptions.Current.YVelocity_High;
                                    redTrailRenderer.SetIsFollowPokeball(true);
                                    break;
                                default:
                                    break;
                            }

                            target = letterPos + POKEBALL_LETTER_CENTER_OFFSET;

                            float velocityZ = target.z - transform.position.z;

                            velocityZ *= Constants.GRAVITY.y;

                            float factor = (-1 * velocityY) - Mathf.Sqrt(Mathf.Pow(velocityY, 2) - (2 * (transform.position.y - target.y) * Constants.GRAVITY.y));
                            factor = Mathf.Pow(factor, -1);

                            velocityZ *= factor;

                            rigidBody.isKinematic = false;

                            Vector3 forceToApply = new Vector3(velocityZ * flickDirection.x * 0.75f, velocityY, velocityZ);

                            if (chargeStrength == ChargeStrength.Low)
                            {
                                forceToApply.x *= SROptions.Current.LowStrengthXFactor;
                            }

                            else if (chargeStrength == ChargeStrength.Medium)
                            {
                                forceToApply.x *= SROptions.Current.MedStrengthXFactor;
                            }

                            else if (chargeStrength == ChargeStrength.High)
                            {
                                forceToApply.x *= SROptions.Current.HighStrengthXFactor;
                            }

                            float halfWidth = (2.0f * (transform.position.z - Camera.main.transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad))
                                * Camera.main.aspect * 0.5f;

                            forceToApply = Quaternion.Euler(0, 1 * (transform.position.x / halfWidth) * Camera.main.fieldOfView, 0) * forceToApply;
                            rigidBody.AddForceAtPosition(forceToApply, transform.position, ForceMode.VelocityChange);

                            yVelocity = velocityY;
                            zVelocity = velocityZ;

                            launchPoint = transform.position;

                            isLaunched = true;
                            timeOfLaunch = Time.time;

                            GameObject ringEffect = (GameObject)Instantiate(ringEffectPrefab, transform.position, Quaternion.Euler(90, 0, 0));
                            ringEffect.SetActive(true);
                            ringEffect.GetComponent<RingEffectController>().Animate(chargeStrength);

                            break;
                        case TouchPhase.Canceled:
                            break;
                        default:
                            break;
                    }
                }

                velocity = (transform.position - lastPosition) / Time.deltaTime;
                lastPosition = transform.position;
            }
        }

        void SetChargeStrength(ChargeStrength chargeStrength)
        {
            if (chargeStrength == this.chargeStrength)
            {
                return;
            }

            //audioSource.Stop();

            switch (chargeStrength)
            {
                case ChargeStrength.None:
                    break;
                case ChargeStrength.Low:
                    //audioSource.PlayOneShot(pokeballLowClip);
                    break;
                case ChargeStrength.Medium:
                    //audioSource.PlayOneShot(pokeballMedClip);
                    break;
                case ChargeStrength.High:
                    //audioSource.PlayOneShot(pokeballHighClip);
                    break;
                default:
                    break;
            }

            this.chargeStrength = chargeStrength;

            ParticleSystemController.instance.OnChargeStrengthUpdate(chargeStrength);


        }


        /*#region Temporary mouse controls
        void OnMouseDown()
        {
            Vector3 mousePosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            touchOffset.x = transform.position.x - mousePosInWorldUnits.x;
            touchOffset.y = transform.position.y - mousePosInWorldUnits.y;
            touchOrigin = mousePosInWorldUnits;
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

            float threshold = 1.5f;
            float unitYDistance = 3.5f;
            float unitYRotation = Mathf.PI * 2;
            float deltaY = position.y - touchOrigin.y;

            flickDirection = new Vector3(0, 0, 0);
            for (int i = 1; i < positions.Count; i++)
            {
                flickDirection += (positions[i] - positions[i - 1]).normalized;
            }
            flickDirection /= positions.Count;
            velocity = (positions[positions.Count - 1] - positions[0]) / Screen.dpi;
            velocity /= times[times.Count - 1] - times[0];

            if (velocity.y > 40)
            {
                SetChargeStrength(ChargeStrength.High);
                rigidBody.angularVelocity = new Vector3(unitYRotation * 6, 0, 0);
            }

            else if (velocity.y > 20 && chargeStrength < ChargeStrength.High)
            {
                SetChargeStrength(ChargeStrength.Medium);
                rigidBody.angularVelocity = new Vector3(unitYRotation * 3.25f, 0, 0);
            }

            else if (velocity.y > 5 && chargeStrength < ChargeStrength.Medium)
            {
                SetChargeStrength(ChargeStrength.Low);
                rigidBody.angularVelocity = new Vector3(unitYRotation * 1.5f, 0, 0);
            }

            else if (chargeStrength < ChargeStrength.Low)
            {
                SetChargeStrength(ChargeStrength.None);
                rigidBody.angularVelocity = new Vector3(0, 0, 0);
            }


        }



        void OnMouseUp()
        {

            flickDirection = new Vector3(0, 0, 0);
            for (int i = 1; i < positions.Count; i++)
            {
                flickDirection += (positions[i] - positions[i - 1]).normalized;
            }
            velocity = (positions[positions.Count - 1] - positions[0]) / Screen.dpi;
            velocity /= times[times.Count - 1] - times[0];
            velocity *= 3;
            flickDirection.Normalize();
            float magnitude = velocity.magnitude;
            magnitude = Mathf.Clamp(magnitude, 20, 100);


            Vector3 letterPos = new Vector3();
            float velocityY = 20f;

            greenTrailRenderer.Reset();
            yellowTrailRenderer.Reset();
            redTrailRenderer.Reset();

            switch (chargeStrength)
            {
                case ChargeStrength.None:
                    Reset();
                    return;
                    break;
                case ChargeStrength.Low:
                    letterPos = ThrowBallsGameManager.Instance.GetPositionOfLetter(0);
                    greenTrailRenderer.SetIsFollowPokeball(true);
                    break;
                case ChargeStrength.Medium:
                    letterPos = ThrowBallsGameManager.Instance.GetPositionOfLetter(1);
                    velocityY = 40;
                    yellowTrailRenderer.SetIsFollowPokeball(true);
                    break;
                case ChargeStrength.High:
                    letterPos = ThrowBallsGameManager.Instance.GetPositionOfLetter(2);
                    velocityY = 60;
                    redTrailRenderer.SetIsFollowPokeball(true);
                    break;
                default:
                    break;
            }

            target = letterPos + POKEBALL_LETTER_CENTER_OFFSET;

            float velocityZ = target.z - transform.position.z;

            velocityZ *= Constants.GRAVITY.y;

            float factor = (-1 * velocityY) - Mathf.Sqrt(Mathf.Pow(velocityY, 2) - (2 * (transform.position.y - target.y) * Constants.GRAVITY.y));
            factor = Mathf.Pow(factor, -1);

            velocityZ *= factor;

            rigidBody.isKinematic = false;

            Vector3 forceToApply = new Vector3(velocityZ * flickDirection.x * 0.75f, velocityY, velocityZ);
            //forceToApply.x = 0;

            float halfWidth = (2.0f * (transform.position.z - Camera.main.transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad))
                * Camera.main.aspect * 0.5f;

            forceToApply = Quaternion.Euler(0, +1 * (transform.position.x / halfWidth) * 22.5f, 0) * forceToApply;
            rigidBody.AddForceAtPosition(forceToApply, transform.position, ForceMode.VelocityChange);

            yVelocity = velocityY;
            zVelocity = velocityZ;

            launchPoint = transform.position;

            isLaunched = true;
            timeOfLaunch = Time.time;
        }

        #endregion*/

    }
}