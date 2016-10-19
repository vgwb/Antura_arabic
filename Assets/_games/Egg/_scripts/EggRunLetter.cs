using UnityEngine;
using DG.Tweening;

namespace EA4S.Egg
{
    public class EggRunLetter
    {
        LetterObjectView letterObject;

        Tween moveTweener;
        Tween rotationTweener;

        Vector3[] outPositions = new Vector3[2];

        int currentOutPosition;

        public EggRunLetter(GameObject letterObjectPrefab, ILivingLetterData letterData, Transform parent, Vector3 leftOutPosition, Vector3 rightOutPosition)
        {
            outPositions[0] = leftOutPosition;
            outPositions[1] = rightOutPosition;

            letterObject = GameObject.Instantiate(letterObjectPrefab).GetComponent<LetterObjectView>();
            letterObject.transform.SetParent(parent);
            letterObject.Init(letterData);

            currentOutPosition = Random.Range(0, 2);

            letterObject.transform.position = outPositions[currentOutPosition];
        }

        public void Run()
        {
            Move(GetNextPosition(), 4f, 0f);
        }

        public void Stop()
        {
            if (moveTweener != null)
            {
                moveTweener.Kill();
            }
        }

        void Move(Vector3 position, float duration, float delay)
        {
            PlayRunAnimation();

            if (moveTweener != null) { moveTweener.Kill(); }

            moveTweener = letterObject.transform.DOMove(position, duration).OnComplete(delegate () { OnMoveComplete(); }).SetDelay(delay);
        }

        void Rotate(Vector3 eulerAngle, float duration, float delay)
        {
            if (rotationTweener != null) { rotationTweener.Kill(); }

            rotationTweener = letterObject.transform.DORotate(eulerAngle, duration).OnComplete(delegate () { OnRotationComplete(); }).SetDelay(delay);
        }

        void OnMoveComplete()
        {
            PlayIdleAnimation();

            Move(GetNextPosition(), 4f, Random.Range(4f, 8f));
        }

        void OnRotationComplete()
        {

        }

        void PlayIdleAnimation()
        {
            letterObject.Model.State = LetterObjectState.LL_idle_1;
        }

        void PlayRunAnimation()
        {
            letterObject.Model.State = LetterObjectState.LL_run_happy;
        }

        public void DestroyRunLetter()
        {
            GameObject.Destroy(letterObject.gameObject);
        }

        Vector3 GetNextPosition()
        {
            Vector3 nextPosition;

            currentOutPosition = Random.Range(0, 2);
            nextPosition = outPositions[currentOutPosition];

            return nextPosition;
        }
    }
}