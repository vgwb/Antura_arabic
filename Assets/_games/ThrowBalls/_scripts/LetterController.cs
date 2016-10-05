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
            Nothing, Bush, Crate, MovingCrate, PileOfCrates
        }

        public enum MotionVariation
        {
            Idle, Jumping, Popping
        }

        public const float JUMP_VELOCITY_IMPULSE = 50f;
        public const float GRAVITY = -245f;
        public const float POPPING_OFFSET = 5f;

        public CrateController crateController;
        public BushController bushController;

        public TMP_Text letterTextView;

        private Animator animator;
        private IEnumerator animationResetter;

        private PropVariation propVariation;
        private MotionVariation motionVariation;

        private float yEquilibrium;

        private LetterData letterData;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetPropVariation(PropVariation propVariation)
        {
            ResetPropVariation();

            switch (propVariation)
            {
                case PropVariation.Crate:
                    crateController.Reset();
                    crateController.Enable();
                    break;
                case PropVariation.MovingCrate:
                    crateController.Reset();
                    crateController.Enable();
                    crateController.SetMoving(Random.Range(0, 50) < 25);
                    break;
                case PropVariation.Bush:
                    bushController.Reset();
                    bushController.Enable();
                    break;
                default:
                    break;
            }

            this.propVariation = propVariation;
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

            this.motionVariation = motionVariation;
        }

        public void ResetPropVariation()
        {
            crateController.Disable();
            bushController.Disable();
            propVariation = PropVariation.Nothing;
        }

        public void ResetMotionVariation()
        {
            StopAllCoroutines();
            motionVariation = MotionVariation.Idle;
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

        public void OnCollision(Collision collision)
        {
            if (collision.gameObject.tag == "Pokeball")
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
            ResetPropVariation();
            yEquilibrium = transform.position.y;
        }
    }
}
