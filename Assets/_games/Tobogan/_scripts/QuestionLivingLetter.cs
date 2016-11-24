using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using ArabicSupport;

namespace EA4S.Tobogan
{
    public class QuestionLivingLetter : MonoBehaviour
    {
        public bool playWhenDragged = true;
        public Transform livingLetterTransform;
        public BoxCollider boxCollider;

        public LetterObjectView letter;

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
        bool dragging = false;
        Vector3 dragOffset = Vector3.zero;

        public event Action onMouseUpLetter;

        Action endTransformToCallback;

        Transform[] letterPositions;
        int currentPosition;

        Vector3 colliderStartScale;

        void Awake()
        {
            normalPosition = livingLetterTransform.localPosition;

            holdPosition.x = normalPosition.x;
            holdPosition.y = normalPosition.y;

            colliderStartScale = boxCollider.size;
        }

        public void Initialize(Camera tubesCamera, Vector3 upRightMaxPosition, Vector3 downLeftMaxPosition, Transform[] letterPositions)
        {
            this.tubesCamera = tubesCamera;
            this.letterPositions = letterPositions;

            cameraDistance = Vector3.Distance(tubesCamera.transform.position, letterPositions[letterPositions.Length - 1].position);

            minX = downLeftMaxPosition.x;
            maxX = upRightMaxPosition.x;
            minY = downLeftMaxPosition.y;
            maxY = upRightMaxPosition.y;

            EnableCollider(true);
        }

        public void PlayIdleAnimation()
        {
            letter.SetState(LLAnimationStates.LL_idle);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayStillAnimation()
        {
            letter.SetState(LLAnimationStates.LL_still);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayWalkAnimation()
        {
            letter.SetState(LLAnimationStates.LL_walking);
            letter.SetWalkingSpeed(LetterObjectView.WALKING_SPEED);

            livingLetterTransform.localPosition = normalPosition;
        }

        public void PlayHoldAnimation()
        {
            letter.SetState(LLAnimationStates.LL_dragging);

            livingLetterTransform.localPosition = holdPosition;
        }

        public void SetQuestionText(ILivingLetterData livingLetterData)
        {
            letter.Init(livingLetterData);
        }

        public void ClearQuestionText()
        {
            letter.Init(null);
        }

        void MoveTo(Vector3 position, float duration)
        {
            PlayWalkAnimation();

            if (moveTweener != null)
            {
                moveTweener.Kill();
            }

            moveTweener = transform.DOLocalMove(position, duration).OnComplete(delegate () 
            {
                if (letter.Data == null)
                    PlayStillAnimation();
                else
                    PlayIdleAnimation();

                if (endTransformToCallback != null)
                    endTransformToCallback();
            });
        }

        void RoteteTo(Vector3 rotation, float duration)
        {
            if (rotationTweener != null)
            {
                rotationTweener.Kill();
            }

            rotationTweener = transform.DORotate(rotation, duration);
        }

        void TransformTo(Transform transformTo, float duration, Action callback)
        {
            MoveTo(transformTo.localPosition, duration);
            RoteteTo(transformTo.eulerAngles, duration);

            endTransformToCallback = callback;
        }

        public void GoToFirstPostion()
        {
            GoToPosition(0);
        }

        public void GoToPosition(int positionNumber)
        {
            dropLetter = false;

            if (moveTweener != null) { moveTweener.Kill(); }
            if (rotationTweener != null) { rotationTweener.Kill(); }

            currentPosition = positionNumber;

            transform.localPosition = letterPositions[currentPosition].localPosition;
            transform.rotation = letterPositions[currentPosition].rotation;
        }

        public void MoveToNextPosition(float duration, Action callback)
        {
            dropLetter = false;

            if (moveTweener != null) { moveTweener.Kill(); }
            if (rotationTweener != null) { rotationTweener.Kill(); }

            currentPosition++;

            if (currentPosition >= letterPositions.Length)
            {
                currentPosition = 0;
            }

            TransformTo(letterPositions[currentPosition], duration, callback);
        }

        public void OnPointerDown(Vector2 pointerPosition)
        {
            if (!dragging)
            {
                dragging = true;

                var data = letter.Data;

                if (playWhenDragged)
                    ToboganConfiguration.Instance.Context.GetAudioManager().PlayLetterData(data, true);

                Vector3 mousePosition = new Vector3(pointerPosition.x, pointerPosition.y, cameraDistance);
                Vector3 world = tubesCamera.ScreenToWorldPoint(mousePosition);
                dragOffset = world - transform.position;

                OnPointerDrag(pointerPosition);

                PlayHoldAnimation();
            }
        }

        public void OnPointerDrag(Vector2 pointerPosition)
        {
            if (dragging)
            {
                dropLetter = false;

                Vector3 mousePosition = new Vector3(pointerPosition.x, pointerPosition.y, cameraDistance);

                transform.position = tubesCamera.ScreenToWorldPoint(mousePosition);

                transform.position = ClampPositionToStage(transform.position - dragOffset);
            }
        }

        public void OnPointerUp()
        {
            if (dragging)
            {
                dragging = false;
                dropLetter = true;

                PlayIdleAnimation();

                if (onMouseUpLetter != null)
                {
                    onMouseUpLetter();
                }
            }
        }

        void Drop(float delta)
        {
            Vector3 dropPosition = transform.position;

            dropPosition += Physics.gravity * delta;

            transform.position = ClampPositionToStage(dropPosition);
        }

        void Update()
        {
            if (dropLetter)
            {
                Drop(Time.deltaTime);
            }

            boxCollider.size = new Vector3(colliderStartScale.x * letter.Scale, colliderStartScale.y, colliderStartScale.z);
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
