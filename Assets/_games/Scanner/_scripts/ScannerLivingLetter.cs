using UnityEngine;
using System.Collections;
using System;

namespace EA4S.Scanner
{

    public class ScannerLivingLetter : MonoBehaviour
    {

        public enum LLStatus { None, Sliding, StandingOnBelt, RunningFromAntura, Lost, Won, Happy, Sad, Flying, Poofing, Falling };
        public GameObject livingLetter;
        public float slideSpeed = 2f;
        public float flightSpeed = 2f;
        public bool facingCamera;
        public LLStatus status = LLStatus.None;
        private float turnAngle;
        private Vector3 startingPosition;
        private Quaternion startingRotation;

        public Transform fallOffPoint;
        public Transform midPoint;

        public LetterObjectView letterObjectView;
        public GameObject rainbowJet;

        public event Action onReset;
        public event Action onStartFallOff;
        public event Action onFallOff;
        public event Action onPassedMidPoint;

        private Transform originalParent;
        private float fallOffX;
        private float midPointX;
        private bool passedMidPoint = false;

        void Start()
        {
            status = LLStatus.None;
            originalParent = transform.parent;
            letterObjectView = livingLetter.GetComponent<LetterObjectView>();
            startingPosition = transform.position;
            startingRotation = letterObjectView.transform.rotation;

            Reset();
        }

        public void Reset()
        {
            StopAllCoroutines();

            rainbowJet.SetActive(false);

            letterObjectView.transform.rotation = startingRotation;
            transform.position = startingPosition;

            fallOffX = fallOffPoint.position.x;
            midPointX = midPoint.position.x;
            passedMidPoint = false;

            turnAngle = facingCamera ? 180 : 0;
            letterObjectView.SetState(LLAnimationStates.LL_still);
            letterObjectView.Falling = true;
            status = LLStatus.Sliding;
            gameObject.GetComponent<SphereCollider>().enabled = true; // enable feet collider
            gameObject.GetComponent<BoxCollider>().enabled = false; // disable body collider
            onReset();

        }

        // Update is called once per frame
        void Update()
        {
            if (status == LLStatus.Sliding) {
                transform.Translate(slideSpeed * Time.deltaTime, -slideSpeed * Time.deltaTime / 2, 0);
            } else if (status == LLStatus.Lost) {
                status = LLStatus.Poofing;
                StartCoroutine(co_Lost());
            } else if (status == LLStatus.Flying) {
                transform.Translate(Vector2.up * flightSpeed * Time.deltaTime);
            } else if (status == LLStatus.Falling) {
                transform.Translate(Vector2.down * flightSpeed * Time.deltaTime);
            }

            // Debug.Log("Letter: " + transform.position.x + " Fall Off X: " + fallOffX);

            if (livingLetter.transform.position.x > fallOffX) {
                StartCoroutine(co_FallOff());
            } else if (livingLetter.transform.position.x > midPointX && !passedMidPoint) {
                passedMidPoint = true;
                onPassedMidPoint();
            }
        }


        IEnumerator co_FlyAway()
        {
            letterObjectView.DoSmallJump();
            yield return new WaitForSeconds(1f);
            rainbowJet.SetActive(true);
            letterObjectView.DoHorray();
            status = LLStatus.Flying;
            yield return new WaitForSeconds(4f);
            Reset();
        }

        IEnumerator co_Lost()
        {
            letterObjectView.DoAngry();
            yield return new WaitForSeconds(2f);
            //			letterObjectView.DoAngry();
            //			yield return new WaitForSeconds(1f);
            letterObjectView.Poof();
            Reset();
        }

        IEnumerator co_FallOff()
        {
            onStartFallOff();
            transform.parent = originalParent;
            letterObjectView.SetState(LLAnimationStates.LL_idle);
            StartCoroutine(RotateGO(livingLetter, new Vector3(90, 90, 0), 1f));
            yield return new WaitForSeconds(0.5f);
            letterObjectView.Falling = true;
            status = LLStatus.Falling;
            yield return new WaitForSeconds(3f);
            Reset();
            onFallOff();
        }

        void OnMouseUp()
        {
            letterObjectView.SetState(LLAnimationStates.LL_tickling);
        }

        public void RoundLost()
        {
            StopAllCoroutines();
            letterObjectView.SetState(LLAnimationStates.LL_idle);
            status = LLStatus.Lost;
        }

        public void RoundWon()
        {
            StartCoroutine(co_FlyAway());
        }

        public void CorrectMove()
        {
            StopAllCoroutines();
            //			letterObjectView.SetState(LLAnimationStates.LL_idle);
            letterObjectView.DoHorray();
        }

        public void WrongMove()
        {
            StopAllCoroutines();
            letterObjectView.SetState(LLAnimationStates.LL_idle);
            letterObjectView.DoAngry();
        }


        IEnumerator RotateGO(GameObject go, Vector3 toAngle, float inTime)
        {
            var fromAngle = go.transform.rotation;
            var destAngle = Quaternion.Euler(toAngle);
            for (var t = 0f; t < 1; t += Time.deltaTime / inTime) {
                go.transform.rotation = Quaternion.Lerp(fromAngle, destAngle, t);
                yield return null;
            }
        }

        IEnumerator AnimateLL()
        {

            yield return new WaitForSeconds(1f);

            int index = -1;
            LLAnimationStates[] animations =
            {
                LLAnimationStates.LL_idle,
                LLAnimationStates.LL_dancing, 
				//LLAnimationStates.LL_walking
			};

            do {
                int oldIndex = index;
                do {
                    index = UnityEngine.Random.Range(0, animations.Length);
                } while (index == oldIndex);
                letterObjectView.SetState(animations[index]);
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));
            } while (status == LLStatus.StandingOnBelt);
        }

        void OnTriggerEnter(Collider other)
        {
            if (status == LLStatus.Sliding) {
                if (other.tag == ScannerGame.TAG_BELT) {
                    transform.parent = other.transform;
                    status = LLStatus.StandingOnBelt;
                    gameObject.GetComponent<SphereCollider>().enabled = false; // disable feet collider
                    gameObject.GetComponent<BoxCollider>().enabled = true; // enable body collider
                    letterObjectView.Falling = false;
                    StartCoroutine(RotateGO(livingLetter, new Vector3(0, turnAngle, 0), 1f));
                    StartCoroutine(AnimateLL());
                }
            }
        }

    }
}
