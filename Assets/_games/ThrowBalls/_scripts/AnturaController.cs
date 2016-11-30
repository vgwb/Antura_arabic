using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class AnturaController : MonoBehaviour
    {
        private const float MAX_CHASING_SPEED = 17.5f;
        private const float MIN_CHASING_SPEED = 3f;
        private const float CHASING_SQUARED_THRESHOLD = 150f;

        public static AnturaController instance;

        private const float RUNNING_SPEED = 17.5f;
        private const float JUMP_INIT_VELOCITY = 60f;

        private float xVelocity;

        private Vector3 jumpPoint;
        private Vector3 ballOffset;
        private AnturaAnimationController animator;
        private bool ballGrabbed;

        private AnturaModelManager modelManager;
        private Rigidbody rigidBody;

        private enum State
        {
            Chasing, AboutToJump, Jumping, Falling, Landed
        }

        private State state;

        void Awake()
        {
            instance = this;

            modelManager = GetComponent<AnturaModelManager>();
            rigidBody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            animator = GetComponent<AnturaAnimationController>();

            //Reset();

            state = State.Chasing;
            animator.State = AnturaAnimationStates.walking;
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

            xVelocity = Mathf.Sign(ballPosition.x - anturaPosition.x) * RUNNING_SPEED;

            if (xVelocity > 0)
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

            jumpPoint.x = (ballPosition.x - 2f * Mathf.Sign(xVelocity)) - (xVelocity / velocityFactor);

            ballGrabbed = false;

            state = State.AboutToJump;

            ThrowBallsConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.DogBarking);
        }

        void Update()
        {
            switch (state)
            {
                case State.AboutToJump:
                    if ((jumpPoint.x - transform.position.x) * xVelocity <= 0)
                    {
                        rigidBody.AddForce(new Vector3(0, JUMP_INIT_VELOCITY, 0), ForceMode.VelocityChange);

                        animator.OnJumpStart();

                        state = State.Jumping;
                    }

                    break;

                case State.Jumping:
                    if (rigidBody.velocity.y < 0)
                    {
                        animator.OnJumpMaximumHeightReached();

                        state = State.Falling;
                    }

                    break;

                case State.Landed:
                    if (IsOffScreen() && xVelocity * transform.position.x > 0)
                    {
                        if (ballGrabbed)
                        {
                            ballGrabbed = false;
                            GameState.instance.OnBallLost();
                            BallController.instance.Reset();
                        }

                        Disable();
                    }

                    break;
            }
        }

        public void DoneChasing()
        {
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0, 8, -3);

            rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            rigidBody.useGravity = false;
        }

        void FixedUpdate()
        {
            if (state == State.Chasing)
            {
                Vector3 velocity = BallController.instance.transform.position - transform.position;
                float distSquared = velocity.sqrMagnitude;

                velocity.Normalize();

                float t = Mathf.Clamp(distSquared / CHASING_SQUARED_THRESHOLD, 0, 1);

                velocity *= Mathf.Lerp(MIN_CHASING_SPEED, MAX_CHASING_SPEED, t);

                animator.SetWalkingSpeed(t < 1 ? t / 2 : 1);

                Vector3 position = transform.position;
                position += velocity * Time.fixedDeltaTime;
                transform.position = position;

                MathUtils.LerpLookAtPlanar(transform, transform.position - velocity, Time.deltaTime * 4);
            }

            else
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

                Vector3 position = transform.position;
                position.x += xVelocity * Time.fixedDeltaTime;
                transform.position = position;

                if (ballGrabbed)
                {
                    BallController.instance.transform.position = modelManager.Dog_jaw.position + ballOffset;
                }
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == Constants.TAG_POKEBALL)
            {
                if (state == State.Chasing)
                {
                    BallController.instance.rigidBody.AddRelativeForce(new Vector3(Random.Range(-150f, 150f), 150f, Random.Range(-150f, 150f)));
                }

                else if (BallController.instance.IsIdle() && !ballGrabbed)
                {
                    animator.OnJumpGrab();
                    BallController.instance.OnIntercepted();
                    ballOffset = new Vector3(Mathf.Sign(xVelocity) * 4f, 0f, 0f);
                    ballGrabbed = true;
                }
            }

            else if (collision.gameObject.tag == "Ground" && state == State.Falling)
            {
                state = State.Landed;

                animator.OnJumpEnded();
                animator.State = AnturaAnimationStates.walking;

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
            xVelocity = 0;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            ballGrabbed = false;

            animator.State = AnturaAnimationStates.walking;
            animator.SetWalkingSpeed(1f);

            state = State.AboutToJump;
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