using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using ArabicSupport;

namespace EA4S.MakeFriends
{
    public class LivingLetterController : MonoBehaviour
    {
        public GameObject container;
        public Animator animator;
        public Collider letterCollider;
        public Rigidbody body;
        public GameObject angerGraphic;
        public LetterData letterData;
        public LetterObject letterObject;
        public WordData wordData;
        public TMP_Text tmpText;
        [Range(-1, 1)]
        public int entranceDirection;

        //private Vector3 standingPosition;
        //private Vector3 offscreenPosition;
        private Vector3 initialRotation;
        private Vector3 entranceRotation;

        private struct WalkParameters
        {
            public Vector3 from;
            public Vector3 to;
            public Vector3 rotation;
            public float duration;
            public string walkAnimation;
            public string afterWalkAnimation;
            public bool speak;
            public float speakDelay;
            public bool rotateAfterWalk;
            public Vector3 afterWalkRotation;

            public WalkParameters(Vector3 from, Vector3 to, Vector3 rotation, float duration, string walkAnimation, string afterWalkAnimation, bool speak, float speakDelay, bool rotateAfterWalk, Vector3 afterWalkRotation)
            {
                this.from = from;
                this.to = to;
                this.rotation = rotation;
                this.duration = duration;
                this.walkAnimation = walkAnimation;
                this.afterWalkAnimation = afterWalkAnimation;
                this.speak = speak;
                this.speakDelay = speakDelay;
                this.rotateAfterWalk = rotateAfterWalk;
                this.afterWalkRotation = afterWalkRotation;
            }
        }
            

        public void Init(WordData _data)
        {
            wordData = _data;
            var text = ArabicFixer.Fix(_data.Word);
            tmpText.text = text;
        }

        void OnMouseDown()
        {
            SpeakWord();
        }

        #region Public Methods
        public void MakeEntrance(Vector3 offscreenPosition, Vector3 startingPosition, Vector3 entranceRotation, float entranceDuration, float speakDelay, Vector3 afterWalkRotation)
        {
            Walk(offscreenPosition, startingPosition, entranceRotation, entranceDuration, speak: true, speakDelay: speakDelay, rotateAfterWalk: true, afterWalkRotation: afterWalkRotation);
            LookAngry();
        }

        public void MakeFriendlyExit(Vector3 position, Vector3 rotation, float duration)
        {
            var from = transform.position;
            var to = position;

            Walk(from, to, rotation, duration, walkAnimation: "Run");
        }

        public void GoToFriendsZone(FriendsZone zone, bool left)
        {
            var side = left ? zone.left : zone.right;
            var from = zone.entrancePosition;
            var to = side.transform.position;
            var rotation = zone.entranceRotation;
            var duration = zone.entranceDuration;
            var afterWalkRotation = left ? zone.finalRotationLeft : zone.finalRotationRight;

            this.transform.SetParent(side.transform);
            this.transform.localPosition = Vector3.zero;
            Walk(from, to, rotation, duration, walkAnimation: "Run", rotateAfterWalk: true, afterWalkRotation: afterWalkRotation);
        }

        public void MoveAwayAngrily(Vector3 position, Vector3 rotation, float duration)
        {
            var from = transform.position;
            var to = position;

            LookAngry();
            Walk(from, to, rotation, duration);
        }

        public void Celebrate(Vector3 celebrationPosition, Vector3 rotation, float celebrationDuration)
        {
            StopCoroutine("LookAngry_Coroutine");
            angerGraphic.SetActive(false);

            var from = transform.position;
            var to = celebrationPosition;
            var duration = celebrationDuration;

            Walk(from, to, rotation, duration, walkAnimation: "Run", afterWalkAnimation: "Ninja");
        }

        public void SpeakWord()
        {
            if (wordData != null && wordData.Key != null)
            {
                AudioManager.I.PlayWord(wordData.Key);
            }
            if (container != null)
            {
                container.GetComponent<Animator>().SetTrigger("Throb");
            }
        }
        #endregion

        #region Private Methods
        private void LookAngry()
        {
            StopCoroutine("LookAngry_Coroutine");
            StartCoroutine("LookAngry_Coroutine");
        }

        private IEnumerator LookAngry_Coroutine()
        {
            angerGraphic.SetActive(true);
            yield return new WaitForSeconds(2f);
            angerGraphic.SetActive(false);
        }

        private void Walk(Vector3 from, Vector3 to, Vector3 rotation, float duration, string walkAnimation = "Walk", string afterWalkAnimation = "Idle", bool speak = false, float speakDelay = 0f, bool rotateAfterWalk = false, Vector3 afterWalkRotation = default(Vector3))
        {
            var parameters = new WalkParameters(from, to, rotation, duration, walkAnimation, afterWalkAnimation, speak, speakDelay, rotateAfterWalk, afterWalkRotation);
            StopCoroutine("Walk_Coroutine");
            StartCoroutine("Walk_Coroutine", parameters);
        }

        private IEnumerator Walk_Coroutine(WalkParameters parameters)
        {
            var from = parameters.from;
            var to = parameters.to;
            var rotation = parameters.rotation;
            var duration = parameters.duration;
            var walkAnimation = parameters.walkAnimation;
            var afterWalkAnimation = parameters.afterWalkAnimation;
            var speak = parameters.speak;
            var speakDelay = parameters.speakDelay;
            var rotateAfterWalk = parameters.rotateAfterWalk;
            var afterWalkRotation = parameters.afterWalkRotation;

            transform.rotation = Quaternion.Euler(rotation);
            animator.SetTrigger(walkAnimation);

            var interpolant = 0f;
            var lerpProgress = 0f;
            var lerpLength = duration;

            while (lerpProgress < lerpLength)
            {
                transform.localPosition = Vector3.Lerp(from, to, interpolant);
                lerpProgress += Time.deltaTime;
                interpolant = lerpProgress / lerpLength;
                interpolant = Mathf.Sin(interpolant * Mathf.PI * 0.5f);
                yield return new WaitForFixedUpdate();
            }

            animator.SetTrigger(afterWalkAnimation);

            if (speak)
            {
                yield return new WaitForSeconds(speakDelay);
                SpeakWord();
                yield return new WaitForSeconds(0.5f);
            }

            if (rotateAfterWalk)
            {
                var initialRotation = transform.rotation.eulerAngles;
                var finalRotation = afterWalkRotation;

                var rotationInterpolant = 0f;
                var rotationLerpProgress = 0f;
                var rotationLerpLength = 0.5f;

                while (rotationLerpProgress < rotationLerpLength)
                {
                    transform.rotation = Quaternion.Euler(Vector3.Lerp(initialRotation, finalRotation, rotationInterpolant));
                    rotationLerpProgress += Time.deltaTime;
                    rotationInterpolant = rotationLerpProgress / rotationLerpLength;
                    rotationInterpolant = Mathf.Sin(rotationInterpolant * Mathf.PI * 0.5f);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        #endregion
    }
}