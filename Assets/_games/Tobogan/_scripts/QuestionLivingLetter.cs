using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

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

        bool dropLetter;

        public event Action onMouseUpLetter;

        Action endTransformToCallback;

        void Awake()
        {
            normalPosition = livingLetterTransform.localPosition;

            holdPosition.x = normalPosition.x + 1f;
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

        void MoveTo(Vector3 position, float duration)
        {
            PlayWalkAnimation();

            moveTweener = transform.DOLocalMove(position, duration).OnComplete(delegate() { PlayIdleAnimation(); if(endTransformToCallback != null) endTransformToCallback(); });
        }

        void RoteteTo(Vector3 rotation, float duration)
        {
            rotationTweener = transform.DORotate(rotation, duration);
        }

        public void TransformTo(Transform transformTo, float duration, Action callback)
        {
            MoveTo(transformTo.localPosition, duration);
            RoteteTo(transformTo.eulerAngles, duration);

            endTransformToCallback = callback;
        }

        void OnMouseDown()
        {
            dropLetter = false;

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
            dropLetter = false;

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
            dropLetter = true;

            PlayIdleAnimation();

            if(onMouseUpLetter != null)
            {
                onMouseUpLetter();
            }
        }

        void Drop(float delta)
        {
            Vector3 dropPosition = transform.localPosition;

            dropPosition += Physics.gravity * delta;

            transform.localPosition = ClampPositionToStage(dropPosition);
        }

        void Update()
        {
            if(dropLetter)
            {
                Drop(Time.deltaTime);
            }
        }

        Vector3 ClampPositionToStage(Vector3 unclampedPosition)
        {
            Vector3 clampedPosition = unclampedPosition;

            clampedPosition.x = clampedPosition.x < minX ? minX : clampedPosition.x;
            clampedPosition.x = clampedPosition.x > maxX ? maxX : clampedPosition.x;
            clampedPosition.y = clampedPosition.y < minY ? minY : clampedPosition.y;
            clampedPosition.y = clampedPosition.y > maxY ? maxY : clampedPosition.y;

            return clampedPosition;
        }

        public void EnableCollider(bool enable)
        {
            boxCollider.enabled = enable;
        }
    }
}
