using UnityEngine;
using DG.Tweening;
using TMPro;

namespace EA4S.Tobogan
{
    class QuestionLivingLetter : MonoBehaviour
    {
        const string idleAnimation = "idle";
        const string holdAnimation = "hold";
        const string runAnimation = "run";
        const string walkAnimation = "walk";
        const string ninjaAnimation = "ninja";

        public Transform livingLetterTransform;
        public BoxCollider boxCollider;

        public Animator AnturaAnimator;

        public TextMeshPro livingLetterText;

        Tweener moveTweener;
        Tweener rotationTweener;

        Vector3 holdPosition;
        Vector3 normalPosition;

        private float cameraDistance;

        Camera tubesCamera;
        float minX;
        float maxX;
        float minY;
        float maxY;

        void Awake()
        {
            normalPosition = livingLetterTransform.localPosition;

            holdPosition.y = normalPosition.y + 5f;
        }

        public void Initialize(Camera tubesCamera, Vector3 endPosition, Vector3 upRightMaxPosition, Vector3 downLeftMaxPosition)
        {
            this.tubesCamera = tubesCamera;

            cameraDistance = Vector3.Distance(tubesCamera.transform.position, endPosition);

            minX = downLeftMaxPosition.x;
            maxX = upRightMaxPosition.x;
            minY = downLeftMaxPosition.y;
            maxY = upRightMaxPosition.y;

            EnableCollider(true);
        }

        public void PlayIdleAnimation()
        {
            PlayAnimation(idleAnimation);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayWalkAnimation()
        {
            PlayAnimation(walkAnimation);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayHoldAnimation()
        {
            PlayAnimation(holdAnimation);

            livingLetterTransform.localPosition = holdPosition;
        }

        void PlayAnimation(string animation)
        {
            AnturaAnimator.Play(animation);
        }

        public void SetQuestionText(ILivingLetterData livingLetterData)
        {
            if (livingLetterData.DataType == LivingLetterDataType.Letter)
            {
                livingLetterText.text = ArabicAlphabetHelper.GetLetterFromUnicode(((LetterData)livingLetterData).Isolated_Unicode);
            }
            else
            {
                livingLetterText.text = ((WordData)livingLetterData).Word;
            }
        }

        public void ClearQuestionText()
        {
            livingLetterText.text = "";
        }

        public void MoveTo(Vector3 position, float time)
        {

        }

        public void RoteteTo(Vector3 rotation, float time)
        {

        }

        public void TransformTo(Transform transformTo, float time)
        {
            MoveTo(transformTo.position, time);
            RoteteTo(transformTo.eulerAngles, time);
        }

        void OnMouseDown()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cameraDistance;

            Vector3 local = transform.localPosition;
            transform.position = tubesCamera.ScreenToWorldPoint(mousePosition);
            local.x = transform.localPosition.x;
            local.y = transform.localPosition.y - 2.5f;

            transform.localPosition = ClampPositionToStage(local);

            PlayHoldAnimation();
        }

        void OnMouseDrag()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = cameraDistance;

            Vector3 local = transform.localPosition;
            transform.position = tubesCamera.ScreenToWorldPoint(mousePosition);
            local.x = transform.localPosition.x;
            local.y = transform.localPosition.y - 2.5f;

            transform.localPosition = ClampPositionToStage(local);
        }

        void OnMouseUp()
        {
            PlayIdleAnimation();
        }

        public Vector3 ClampPositionToStage(Vector3 unclampedPosition)
        {
            Vector3 clampedPosition = unclampedPosition;

            clampedPosition.x = clampedPosition.x < minX ? minX : clampedPosition.x;
            clampedPosition.x = clampedPosition.x > maxX ? maxX : clampedPosition.x;
            clampedPosition.y = clampedPosition.y < minY ? minY : clampedPosition.y;
            clampedPosition.y = clampedPosition.y > maxY ? maxY : clampedPosition.y;

            return clampedPosition;
        }

        void EnableCollider(bool enable)
        {
            boxCollider.enabled = enable;
        }
    }
}
