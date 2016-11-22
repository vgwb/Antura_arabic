using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class AnturaController : MonoBehaviour
    {
        public static AnturaController instance;

        private const float RUNNING_SPEED = 17.5f;
        private const float JUMP_INIT_VELOCITY = 50f;

        private Vector3 velocity;
        private Vector3 jumpPoint;
        private Vector3 ballOffset;
        private AnturaAnimationController animator;
        private bool jumped;
        private bool landed;
        private bool reachedJumpMaxNotified;
        private bool ballGrabbed;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            animator = GetComponent<AnturaAnimationController>();

            Reset();
        }

        public void EnterScene()
        {
            Vector3 ballPosition = BallController.instance.transform.position;
            Vector3 anturaPosition = ballPosition;
            anturaPosition.y = GroundController.instance.transform.position.y;

            float frustumHeight = 2.0f * Mathf.Abs(anturaPosition.z - Camera.main.transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * Camera.main.aspect;

            anturaPosition.x = frustumWidth / 2 + 2f;

            if (Random.Range(0, 40) % 2 == 0)
            {
                anturaPosition.x *= -1;
            }

            velocity.x = ballPosition.x - anturaPosition.x;
            velocity.Normalize();
            velocity *= RUNNING_SPEED;

            if (velocity.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 270, 0);
            }

            else
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            transform.position = anturaPosition;
            jumpPoint = anturaPosition;

            float velocityFactor = (-1 * JUMP_INIT_VELOCITY) - Mathf.Sqrt(Mathf.Pow(JUMP_INIT_VELOCITY, 2) + (2 * (anturaPosition.y + 4f - ballPosition.y) * Constants.GRAVITY.y));
            velocityFactor = Mathf.Pow(velocityFactor, -1);
            velocityFactor *= Constants.GRAVITY.y;

            jumpPoint.x = (ballPosition.x - 2f * Mathf.Sign(velocity.x)) - (velocity.x / velocityFactor);

            jumped = false;
            landed = false;
            reachedJumpMaxNotified = false;
            ballGrabbed = false;
        }

        void Update()
        {
            Vector3 position = transform.position;

            if (jumped && !landed)
            {
                velocity.y += Time.deltaTime * Constants.GRAVITY.y;

                if (position.y < GroundController.instance.transform.position.y)
                {
                    position.y = GroundController.instance.transform.position.y;
                    velocity.y = 0;
                    landed = true;

                    animator.OnJumpEnded();
                }

                else if (velocity.y < 0 && !reachedJumpMaxNotified)
                {
                    animator.OnJumpMaximumHeightReached();
                    reachedJumpMaxNotified = true;
                }
            }

            position += velocity * Time.deltaTime;
            transform.position = position;

            if ((jumpPoint.x - position.x) * velocity.x <= 0 && !jumped)
            {
                velocity.y = JUMP_INIT_VELOCITY;

                jumped = true;
                
                animator.OnJumpStart();
            }

            if (ballGrabbed)
            {
                BallController.instance.transform.position = transform.position + ballOffset;
            }

            if (IsOffScreen() && velocity.x * transform.position.x > 0)
            {
                if (ballGrabbed)
                {
                    ballGrabbed = false;
                    ThrowBallsGameManager.Instance.OnBallLost();
                    BallController.instance.Reset();
                }

                Disable();
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == Constants.TAG_POKEBALL && !ballGrabbed)
            {
                animator.OnJumpGrab();
                BallController.instance.OnIntercepted();
                ballOffset = new Vector3(Mathf.Sign(velocity.x) * 7.56f, 2f, 0f);
                ballGrabbed = true;
            }
        }

        private bool IsOffScreen()
        {
            float frustumHeight = 2.0f * Mathf.Abs(transform.position.z - Camera.main.transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * Camera.main.aspect;
            float halfFrustumWidth = frustumWidth / 2;

            if (Mathf.Abs(transform.position.x) - 12f > halfFrustumWidth)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public void Reset()
        {
            velocity = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            jumped = false;
            landed = false;
            reachedJumpMaxNotified = false;
            ballGrabbed = false;
            
            animator.State = AnturaAnimationStates.walking;
            animator.SetWalkingSpeed(1f);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}