using UnityEngine;
using System.Collections;
using TMPro;
using EA4S;

namespace EA4S.ThrowBalls
{
    public class LetterController : MonoBehaviour
    {
        public enum PropVariation
        {
            Nothing, Bush, SwervingPileOfCrates, StaticPileOfCrates
        }

        public enum MotionVariation
        {
            Idle, Jumping, Popping
        }

        public const float JUMP_VELOCITY_IMPULSE = 50f;
        public const float GRAVITY = -245f;
        public const float POPPING_OFFSET = 5f;
        public const float PROP_UP_TIME = 0.2f;
        public const float PROP_UP_DELAY = 0.3f;

        public CratePileController cratePileController;
        public BushController bushController;

        public TMP_Text letterTextView;

        private Animator animator;
        private IEnumerator animationResetter;

        private float yEquilibrium;

        private LetterData letterData;

        private IEnumerator customGravityCoroutine;
        private IEnumerator propUpCoroutine;

        public Rigidbody rigidBody;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetPropVariation(PropVariation propVariation)
        {
            ResetProps();
            DisableProps();

            switch (propVariation)
            {
                case PropVariation.StaticPileOfCrates:
                    cratePileController.Enable();
                    break;
                case PropVariation.SwervingPileOfCrates:
                    cratePileController.Enable();
                    cratePileController.SetSwerving();
                    break;
                case PropVariation.Bush:
                    bushController.Enable();
                    break;
                default:
                    break;
            }
        }

        public void SetMotionVariation(MotionVariation motionVariation)
        {
            ResetMotionVariation();

            switch (motionVariation)
            {
                case MotionVariation.Idle:
                    break;
                case MotionVariation.Jumping:
                    SetIsJumping();
                    break;
                case MotionVariation.Popping:
                    SetIsPoppingUpAndDown();
                    break;
                default:
                    break;
            }
        }

        public void ResetProps()
        {
            cratePileController.Reset();
            bushController.Reset();
        }

        public void DisableProps()
        {
            cratePileController.Disable();
            bushController.Disable();
        }

        public void ResetMotionVariation()
        {
            StopAllCoroutines();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetLetter(LetterData _data)
        {
            letterData = _data;
            letterTextView.text = letterData.TextForLivingLetter;
        }

        public LetterData GetLetter()
        {
            return letterData;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == Constants.TAG_POKEBALL)
            {
                if (tag == Constants.TAG_CORRECT_LETTER)
                {
                    ThrowBallsGameManager.Instance.OnCorrectLetterHit(this);
                }

                else
                {
                    animator.Play("run");
                    if (animationResetter != null)
                    {
                        StopCoroutine(animationResetter);
                    }
                    animationResetter = ResetAnimation();
                    StartCoroutine(animationResetter);
                }
            }

            else if (collision.gameObject.tag == Constants.TAG_RAIL)
            {
                rigidBody.isKinematic = true;
                PropUp(PROP_UP_DELAY);
                Debug.Log("Propping Up");
            }
        }

        public void MoveBy(float deltaX, float deltaY, float deltaZ)
        {
            Vector3 position = transform.position;
            position.x += deltaX;
            position.y += deltaY;
            position.z += deltaZ;
            transform.position = position;

            yEquilibrium += deltaY;
        }

        public void MoveTo(float x, float y, float z)
        {
            Vector3 position = transform.position;
            position.x = x;
            position.y = y;
            position.z = z;
            transform.position = position;
        }

        private void SetIsJumping()
        {
            StartCoroutine("Jump");
        }

        private IEnumerator Jump()
        {
            yield return new WaitForSeconds(Random.Range(0.25f, 1f));

            for (;;)
            {
                yEquilibrium = transform.position.y;
                float yVelocity = JUMP_VELOCITY_IMPULSE;

                float yDelta = 0;

                while (yVelocity > 0 || (yVelocity < 0 && !PassesEquilibriumOnNextFrame(yVelocity, yDelta, yEquilibrium)))
                {
                    yVelocity += GRAVITY * Time.fixedDeltaTime;
                    yDelta = yVelocity * Time.fixedDeltaTime;

                    transform.position = new Vector3(transform.position.x, transform.position.y + yDelta, transform.position.z);

                    yield return new WaitForFixedUpdate();
                }

                transform.position = new Vector3(transform.position.x, yEquilibrium, transform.position.z);

                yield return new WaitForSeconds(1.5f);
            }
        }

        public void SetIsDropping()
        {
            StartCoroutine("Drop");
        }

        private IEnumerator Drop()
        {
            yEquilibrium -= 4.2f;
            float yVelocity = -0.001f;

            float yDelta = 0;

            float DROP_GRAVITY = -100;

            while (yVelocity > 0 || (yVelocity < 0 && !PassesEquilibriumOnNextFrame(yVelocity, yDelta, yEquilibrium)))
            {
                yVelocity += DROP_GRAVITY * Time.fixedDeltaTime;
                yDelta = yVelocity * Time.fixedDeltaTime;

                transform.position = new Vector3(transform.position.x, transform.position.y + yDelta, transform.position.z);

                yield return new WaitForFixedUpdate();
            }

            transform.position = new Vector3(transform.position.x, yEquilibrium, transform.position.z);
        }

        private void PropUp(float delay)
        {
            propUpCoroutine = PropUpCoroutine(delay);
            StartCoroutine(propUpCoroutine);
        }

        private IEnumerator PropUpCoroutine(float delay)
        {
            float initZAngle = transform.localRotation.eulerAngles.z;

            if (initZAngle > 180)
            {
                initZAngle = 180 - initZAngle;
            }

            float initZAngleSign = Mathf.Sign(initZAngle);
            float propUpSpeed = (initZAngle / PROP_UP_TIME) * initZAngleSign * -1;

            yield return new WaitForSeconds(delay);

            while (true)
            {
                transform.RotateAround(transform.position, Vector3.back, propUpSpeed * Time.fixedDeltaTime);

                float currentZAngle = transform.localRotation.eulerAngles.z;

                if (currentZAngle > 180)
                {
                    currentZAngle = 180 - currentZAngle;
                }

                if (Mathf.Sign(currentZAngle) * initZAngleSign <= -1)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);

                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private bool PassesEquilibriumOnNextFrame(float velocity, float deltaPos, float equilibrium)
        {
            return (velocity < 0 && transform.position.y + deltaPos < equilibrium)
                    || (velocity > 0 && transform.position.y + deltaPos > equilibrium);
        }


        private void SetIsPoppingUpAndDown()
        {
            StartCoroutine("PopUpAndDown");
        }

        private IEnumerator PopUpAndDown()
        {
            yield return new WaitForSeconds(Random.Range(0.25f, 1f));

            bool isPoppingUp = true;

            for (;;)
            {
                yEquilibrium = isPoppingUp ? transform.position.y + POPPING_OFFSET : transform.position.y - POPPING_OFFSET;
                float yVelocity = isPoppingUp ? JUMP_VELOCITY_IMPULSE : -JUMP_VELOCITY_IMPULSE;

                float yDelta = 0;

                while (!PassesEquilibriumOnNextFrame(yVelocity, yDelta, yEquilibrium))
                {
                    yDelta = yVelocity * Time.fixedDeltaTime;

                    transform.position = new Vector3(transform.position.x, transform.position.y + yDelta, transform.position.z);

                    yield return new WaitForFixedUpdate();
                }

                transform.position = new Vector3(transform.position.x, yEquilibrium, transform.position.z);

                isPoppingUp = !isPoppingUp;

                yield return new WaitForSeconds(0.75f);
            }
        }

        public void ApplyCustomGravity()
        {
            customGravityCoroutine = ApplyCustomGravityCoroutine();
            StartCoroutine(customGravityCoroutine);
        }

        private IEnumerator ApplyCustomGravityCoroutine()
        {
            rigidBody.isKinematic = false;

            while (true)
            {
                rigidBody.AddForce(Constants.GRAVITY_FACTOR * Physics.gravity, ForceMode.Acceleration);

                yield return new WaitForFixedUpdate();
            }
        }

        public void StopCustomGravity()
        {
            StopCoroutine(customGravityCoroutine);
        }

        public void Show()
        {
            GameObject poof = (GameObject)Instantiate(ThrowBallsGameManager.Instance.poofPrefab, transform.position, Quaternion.identity);
            Destroy(poof, 10);
            gameObject.SetActive(true);
        }

        public void Vanish()
        {
            GameObject poof = (GameObject)Instantiate(ThrowBallsGameManager.Instance.poofPrefab, transform.position, Quaternion.identity);
            Destroy(poof, 10);
            gameObject.SetActive(false);
        }

        private IEnumerator ResetAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            animator.Play("idle");
            animationResetter = null;
        }

        public void Reset()
        {
            ResetMotionVariation();
            ResetProps();
            DisableProps();
            yEquilibrium = transform.position.y;
            transform.rotation = Quaternion.Euler(0, 180, 0);
            rigidBody.isKinematic = true;
        }
    }
}
