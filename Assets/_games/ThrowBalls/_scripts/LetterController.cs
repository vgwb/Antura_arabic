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
        public BoxCollider boxCollider;

        public LetterWithPropsController letterWithPropsCntrl;

        private Vector3 lastPosition;
        private Vector3 lastRotation;

        private bool isGrounded = false;
        private bool isProppingUp = false;
        private bool isStill = false;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            SetIsColliderEnabled(true);
        }

        public void SetPropVariation(PropVariation propVariation)
        {
            letterWithPropsCntrl.AccountForProp(propVariation);

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
            Vector3 currentPosition = transform.position;
            Vector3 currentRotation = transform.rotation.eulerAngles;

            if (AreVectorsApproxEqual(currentPosition, lastPosition) && AreVectorsApproxEqual(currentRotation, lastRotation))
            {
                isStill = true;
            }

            else
            {
                isStill = false;
            }

            lastPosition = currentPosition;
            lastRotation = currentRotation;
        }

        private bool AreVectorsApproxEqual(Vector3 vector1, Vector3 vector2)
        {
            return Mathf.Approximately(vector1.x, vector2.x) && Mathf.Approximately(vector1.y, vector2.y) && Mathf.Approximately(vector1.z, vector2.z);
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
                if (propUpCoroutine == null)
                {
                    //SetIsKinematic(true);
                    //PropUp(PROP_UP_DELAY);
                }

                isGrounded = true;
            }

            if (isProppingUp)
            {
                isProppingUp = false;

                if (propUpCoroutine != null)
                {
                    StopCoroutine(propUpCoroutine);
                }
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == Constants.TAG_RAIL)
            {
                isGrounded = false;
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

        public void MakeSureIsProppedUp()
        {
            StartCoroutine(MakeSureIsProppedUpCoroutine());
        }

        private IEnumerator MakeSureIsProppedUpCoroutine()
        {
            while (true)
            {
                if (isGrounded && isStill)
                {
                    SetIsKinematic(true);

                    if (transform.rotation.eulerAngles.z != 0 && !isProppingUp)
                    {
                        PropUp(0.2f);
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public void PropUp(float delay)
        {
            isProppingUp = true;
            propUpCoroutine = PropUpCoroutine(delay);
            StartCoroutine(propUpCoroutine);
        }

        private IEnumerator PropUpCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            float initZAngle = transform.rotation.eulerAngles.z;

            if (initZAngle > 180)
            {
                initZAngle = 180 - initZAngle;
            }

            float initZAngleSign = Mathf.Sign(initZAngle);
            float propUpSpeed = (initZAngle / PROP_UP_TIME) * initZAngleSign * -1;

            SetIsKinematic(true);
            SetIsColliderEnabled(false);

            Vector3 centerOfRotation = boxCollider.transform.position;

            while (true)
            {
                transform.RotateAround(centerOfRotation, initZAngleSign > 0 ? Vector3.back : Vector3.forward, propUpSpeed * Time.fixedDeltaTime);

                float currentZAngle = transform.rotation.eulerAngles.z;

                if (currentZAngle > 180)
                {
                    currentZAngle = 180 - currentZAngle;
                }

                if (Mathf.Sign(currentZAngle) * initZAngleSign <= -1)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    SetIsColliderEnabled(true);
                    isProppingUp = false;
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public void SetIsKinematic(bool isKinematic)
        {
            rigidBody.isKinematic = isKinematic;
        }

        public void SetIsColliderEnabled(bool isEnabled)
        {
            boxCollider.enabled = isEnabled;
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
            while (true)
            {
                rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);

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
            propUpCoroutine = null;
            SetIsKinematic(true);
            isProppingUp = false;
            isGrounded = false;
            isStill = false;
            SetIsColliderEnabled(true);
        }
    }
}
