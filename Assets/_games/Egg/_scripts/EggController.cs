using DG.Tweening;
using System;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggController : MonoBehaviour
    {
        public EggLivingLetter eggLivingLetter;
        public GameObject egg;

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
            currentRotation = Vector3.zero;
            GoToPosition(0, currentRotation);

            eggLivingLetter.gameObject.SetActive(false);
            egg.gameObject.SetActive(true);
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

        public void Cracking()
        {

        }

        public void Crack()
        {
            eggLivingLetter.gameObject.SetActive(true);
            egg.gameObject.SetActive(false);
        }

        void MoveTo(Vector3 position, float duration)
        {
            if (moveTweener != null)
            {
                moveTweener.Kill();
            }

            moveTweener = transform.DOLocalMove(position, duration).OnComplete(delegate () { if (endTransformToCallback != null) endTransformToCallback(); });
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
    }
}