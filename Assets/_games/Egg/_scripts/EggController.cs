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

        Tween moveTweener;
        Tween rotationTweener;

        Transform eggTransform;

        Action endTransformToCallback;

        int currentPosition;
        Vector3 currentRotation;

        Vector3[] eggPositions;

        public void Initialize(Vector3[] eggPositions)
        {
            this.eggPositions = eggPositions;
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
            if (moveTweener != null) { moveTweener.Kill(); }
            if (rotationTweener != null) { rotationTweener.Kill(); }

            currentPosition++;

            if (currentPosition >= eggPositions.Length)
            {
                currentPosition = 0;
            }

            currentRotation.z += 90f;

            TransformTo(eggPositions[currentPosition], currentRotation, duration, callback);
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
            egg.gameObject.SetActive(false);

            if (onEggCrackComplete != null)
            {
                onEggCrackComplete();
            }
        }

        void MoveTo(Vector3 position, float duration)
        {
            if (moveTweener != null)
            {
                moveTweener.Kill();
            }

            moveTweener = transform.DOLocalMove(position, duration).OnComplete(delegate ()
            {
                if (endTransformToCallback != null) endTransformToCallback();

                if (onEggExitComplete != null && (currentPosition == eggPositions.Length - 1))
                {
                    onEggExitComplete();
                }

            });
        }

        void RoteteTo(Vector3 rotation, float duration)
        {
            if (rotationTweener != null)
            {
                rotationTweener.Kill();
            }

            rotationTweener = egg.transform.DORotate(rotation, duration);
        }

        void TransformTo(Vector3 localPosition, Vector3 rotation, float duration, Action callback)
        {
            MoveTo(localPosition, duration);
            RoteteTo(rotation, duration);

            endTransformToCallback = callback;
        }

        void GoToPosition(int positionNumber, Vector3 rotation)
        {
            if (moveTweener != null) { moveTweener.Kill(); }
            if (rotationTweener != null) { rotationTweener.Kill(); }

            currentPosition = positionNumber;

            transform.localPosition = eggPositions[currentPosition];
            egg.transform.eulerAngles = rotation;
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