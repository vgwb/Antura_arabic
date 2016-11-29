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

        private float yEquilibrium;

        private ILivingLetterData letterData;

        private IEnumerator customGravityCoroutine;
        private IEnumerator propUpCoroutine;
        private IEnumerator jumpCoroutine;

        public Rigidbody rigidBody;
        public BoxCollider boxCollider;

        public LetterWithPropsController letterWithPropsCntrl;

        private Vector3 lastPosition;
        private Vector3 lastRotation;

        private bool isGrounded = false;
        private bool isProppingUp = false;
        private bool isStill = false;

        private MotionVariation motionVariation;
        
        public GameObject shadow;

        [HideInInspector]
        public LetterObjectView letterObjectView;

        public GameObject victoryRays;

        // Use this for initialization
        void Start()
        {
            letterObjectView = GetComponent<LetterObjectView>();

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
                    shadow.SetActive(false);
                    break;
                case PropVariation.SwervingPileOfCrates:
                    cratePileController.Enable();
                    cratePileController.SetSwerving();
                    shadow.SetActive(false);
                    break;
                case PropVariation.Bush:
                    bushController.Enable();
                    shadow.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void SetMotionVariation(MotionVariation motionVariation)
        {
            ResetMotionVariation();

            this.motionVariation = motionVariation;

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
            Vector3 currentRotation = transform.localRotation.eulerAngles;

            if (AreVectorsApproxEqual(currentPosition, lastPosition, 0.0001f) && AreVectorsApproxEqual(currentRotation, lastRotation, 0.1f))
            {
                isStill = true;
            }

            else
            {
                isStill = false;
            }

            //Debug.Log(currentRotation.x + "," + lastRotation.x);

            lastPosition = currentPosition;
            lastRotation = currentRotation;
        }

        private bool AreVectorsApproxEqual(Vector3 vector1, Vector3 vector2, float threshold)
        {
            return Mathf.Abs(vector1.x - vector2.x) <= threshold && Mathf.Abs(vector1.y - vector2.y) <= threshold && Mathf.Abs(vector1.z - vector2.z) <= threshold;
        }

        public void SetLetter(ILivingLetterData _data)
        {
            letterData = _data;
            letterObjectView.Init(letterData);
        }

        public ILivingLetterData GetLetter()
        {
            return letterData;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == Constants.TAG_POKEBALL)
            {
                if (tag == Constants.TAG_CORRECT_LETTER)
                {
                    GameState.instance.OnCorrectLetterHit(this);
                    ThrowBallsConfiguration.Instance.Context.GetAudioManager().PlaySound(Sfx.Poof);
                }

                else
                {
                    letterObjectView.DoTwirl(null);
                    BallController.instance.OnRebounded();
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
            jumpCoroutine = Jump();
            StartCoroutine(jumpCoroutine);
        }

        private IEnumerator Jump()
        {
            yield return new WaitForSeconds(Random.Range(0.4f, 1f));

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

        public void StopJumping()
        {
            StopCoroutine(jumpCoroutine);
        }

        public bool IsJumping()
        {
            return motionVariation == MotionVariation.Jumping;
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

        public void MakeSureIsProppedUp(float delay)
        {
            StartCoroutine(MakeSureIsProppedUpCoroutine(delay));
        }

        private IEnumerator MakeSureIsProppedUpCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            while (true)
            {
                //Debug.Log(isGrounded + "," + isStill);
                if (isGrounded && isStill)
                {
                    SetIsKinematic(true);

                    if (transform.rotation.eulerAngles.z != 0 && !isProppingUp)
                    {
                        PropUp(0.2f);
                    }

                    if (Mathf.Approximately(transform.rotation.eulerAngles.z, 0) && !isProppingUp)
                    {
                        shadow.SetActive(true);
                    }
                }

                else
                {
                    yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
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

            StopCustomGravity();

            float initZAngle = transform.rotation.eulerAngles.z;

            if (initZAngle > 180)
            {
                initZAngle = 180 - initZAngle;
            }

            float initZAngleSign = Mathf.Sign(initZAngle);
            float propUpSpeed = (initZAngle / PROP_UP_TIME) * initZAngleSign * -1;

            //SetIsColliderEnabled(false);

            Vector3 centerOfRotation = boxCollider.transform.position;

            while (true)
            {
                transform.RotateAround(centerOfRotation, initZAngleSign > 0 ? Vector3.back : Vector3.forward, propUpSpeed * Time.fixedDeltaTime);

                float currentZAngle = transform.localRotation.eulerAngles.z;

                if (currentZAngle > 180)
                {
                    currentZAngle = 180 - currentZAngle;
                }

                if (Mathf.Sign(currentZAngle) * initZAngleSign <= -1 || Mathf.Abs(currentZAngle) <= 0.05)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    SetIsColliderEnabled(true);
                    isProppingUp = false;

                    if (motionVariation == MotionVariation.Jumping)
                    {
                        SetIsJumping();
                    }

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

            IAudioManager audioManager = ThrowBallsConfiguration.Instance.Context.GetAudioManager();

            for (;;)
            {
                yEquilibrium = isPoppingUp ? transform.position.y + POPPING_OFFSET : transform.position.y - POPPING_OFFSET;
                float yVelocity = isPoppingUp ? JUMP_VELOCITY_IMPULSE : -JUMP_VELOCITY_IMPULSE;

                float yDelta = 0;

                if (isPoppingUp)
                {
                    audioManager.PlaySound(Sfx.BushRustlingIn);
                }

                else
                {
                    audioManager.PlaySound(Sfx.BushRustlingOut);
                }

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
            GameObject poof = (GameObject)Instantiate(ThrowBallsGame.instance.poofPrefab, transform.position, Quaternion.identity);
            Destroy(poof, 10);
            gameObject.SetActive(true);
        }

        public void Vanish()
        {
            GameObject poof = (GameObject)Instantiate(ThrowBallsGame.instance.poofPrefab, transform.position, Quaternion.identity);
            Destroy(poof, 10);
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            ResetMotionVariation();
            ResetProps();
            DisableProps();
            yEquilibrium = transform.position.y;
            transform.rotation = Quaternion.Euler(0, 180, 0);
            propUpCoroutine = null;
            SetIsKinematic(true);
            isProppingUp = false;
            isGrounded = false;
            isStill = false;
            SetIsColliderEnabled(true);
            shadow.SetActive(true);
            victoryRays.SetActive(false);
            letterObjectView.SetState(LLAnimationStates.LL_idle);
        }

        public void ShowVictoryRays()
        {
            victoryRays.SetActive(true);
        }

        void OnMouseDown()
        {
            if (GameState.instance.isRoundOngoing)
            {
                AudioManager.I.PlayLetter(letterData.Id);
            }
        }
    }
}
