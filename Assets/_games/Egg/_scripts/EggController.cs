using DG.Tweening;
using System;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggController : MonoBehaviour
    {
        public EggLivingLetter eggLivingLetter;
        public GameObject egg;

        public Collider eggCollider;

        public TremblingTube tremblingEgg;
        float tremblingTimer;

        public Action onEggCrackComplete;
        public Action onEggExitComplete;

        Tween moveEggTween;
        Tween rotationEggTween;

        Transform eggTransform;

        Action endTransformToCallback;

        int currentPosition;
        Vector3 currentRotation;

        Vector3[] eggPositions;

        public void Initialize(GameObject letterObjectViewPrefab,  Vector3[] eggPositions)
        {
            this.eggPositions = eggPositions;
            eggLivingLetter.Initialize(letterObjectViewPrefab);
        }

        public void Reset()
        {
            currentRotation = new Vector3(0f, 0f, -90f);
            GoToPosition(0, currentRotation);

            eggLivingLetter.gameObject.SetActive(false);
            egg.gameObject.SetActive(true);

            tremblingTimer = 0f;
        }

        public void MoveNext(float duration, Action callback)
        {
            if (moveEggTween != null) { moveEggTween.Kill(); }
            if (rotationEggTween != null) { rotationEggTween.Kill(); }

            currentPosition++;

            if (currentPosition >= eggPositions.Length)
            {
                currentPosition = 0;
            }

            currentRotation.z += 90f;

            bool inOutRotation = currentPosition == 1 || currentPosition == eggPositions.Length - 1;

            TransformTo(eggPositions[currentPosition], inOutRotation, currentRotation, duration, callback);
        }

        public bool isNextToExit
        {
            get
            {
                if (currentPosition == eggPositions.Length - 2)
                    return true;

                return false;
            }

        }

        public void ResetCrack()
        {

        }

        public void Cracking(float progress)
        {
            StartTrembling();

            if (progress == 1f)
            {
                Crack();
            }
        }

        public void Crack()
        {
            eggLivingLetter.gameObject.SetActive(true);
            eggLivingLetter.PlayIdleAnimation();

            egg.gameObject.SetActive(false);

            if (onEggCrackComplete != null)
            {
                onEggCrackComplete();
            }
        }

        void MoveTo(Vector3 position, float duration)
        {
            if (moveEggTween != null)
            {
                moveEggTween.Kill();
            }

            moveEggTween = transform.DOLocalMove(position, duration).OnComplete(delegate ()
            {
                if (endTransformToCallback != null) endTransformToCallback();

                if (onEggExitComplete != null && (currentPosition == eggPositions.Length - 1))
                {
                    onEggExitComplete();
                }

            });
        }

        void InOutRotation(Vector3 rotation, float duration)
        {
            if (rotationEggTween != null)
            {
                rotationEggTween.Kill();
            }

            rotationEggTween = DOTween.To(() => egg.transform.eulerAngles.z, z => egg.transform.eulerAngles = new Vector3(egg.transform.eulerAngles.x, egg.transform.eulerAngles.y, z), rotation.z + 1080f, duration * 0.95f)
                .OnComplete(delegate ()
                {
                    BouncingRotation(0.5f);
                });
        }

        void RoteteTo(Vector3 rotation, float duration)
        {
            if (rotationEggTween != null)
            {
                rotationEggTween.Kill();
            }

            rotationEggTween = egg.transform.DORotate(rotation, duration * 0.93f).OnComplete(delegate()
            {
                BouncingRotation();
            });
        }

        void BouncingRotation(float duration = 0.8f)
        {
            float firstStepValue = 5f;
            float secondStepValue = -2.5f;

            Vector3 rotationFirstStep = Vector3.zero;
            rotationFirstStep.z += firstStepValue;
            Vector3 rotationSecondStep = Vector3.zero;
            rotationSecondStep.z += secondStepValue;

            rotationEggTween = transform.DORotate(rotationFirstStep, (duration / 10f) * 5f).OnComplete(delegate ()
            {
                rotationEggTween = transform.DORotate(rotationSecondStep, (duration / 10f) * 4f).OnComplete(delegate ()
                {
                    rotationEggTween = transform.DORotate(Vector3.zero, (duration / 10f) * 2f);
                });
            });
        }

        void TransformTo(Vector3 localPosition, bool inOutRotation, Vector3 rotation, float duration, Action callback)
        {
            MoveTo(localPosition, duration);
            if(inOutRotation)
            {
                InOutRotation(rotation, duration);
            }
            else
            {
                RoteteTo(rotation, duration);
            }

            endTransformToCallback = callback;
        }

        void GoToPosition(int positionNumber, Vector3 rotation)
        {
            if (moveEggTween != null) { moveEggTween.Kill(); }
            if (rotationEggTween != null) { rotationEggTween.Kill(); }

            currentPosition = positionNumber;

            transform.localPosition = eggPositions[currentPosition];
            egg.transform.eulerAngles = rotation;
            transform.eulerAngles = Vector3.zero;
        }

        public void EnableInput()
        {
            eggCollider.enabled = true;
        }

        public void DisableInput()
        {
            eggCollider.enabled = false;
        }

        void Update()
        {
            if (tremblingTimer > 0f)
            {
                tremblingTimer -= Time.deltaTime;
                tremblingEgg.Trembling = true;
            }
            else
            {
                tremblingEgg.Trembling = false;
            }
        }

        public void StartTrembling()
        {
            tremblingTimer = 0.5f;
        }
    }
}