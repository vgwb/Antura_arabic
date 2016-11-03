using System;
using UnityEngine;
using DG.Tweening;

namespace EA4S.Egg
{
    public class EggLivingLetter
    {
        LetterObjectView livingLetter;

        Vector3 startPosition;
        Vector3 endPosition;

        float delay;

        Action endCallback;

        public EggLivingLetter(Transform parent, GameObject letterObjectViewPrefab, ILivingLetterData livingLetterData, Vector3 startPosition, Vector3 endPosition, float delay, Action endCallback)
        {
            livingLetter = UnityEngine.Object.Instantiate(letterObjectViewPrefab).GetComponent<LetterObjectView>();

            livingLetter.transform.SetParent(parent);
            livingLetter.transform.localPosition = startPosition;
            livingLetter.Init(livingLetterData);
            livingLetter.gameObject.SetActive(false);

            this.startPosition = startPosition;
            this.endPosition = endPosition;

            this.delay = delay;

            this.endCallback = endCallback;

            JumpToEnd();
        }

        public void PlayHorrayAnimation()
        {
            livingLetter.DoHorray();
        }

        public void DestroyLetter()
        {
            UnityEngine.Object.Destroy(livingLetter.gameObject);
        }

        public void JumpToEnd()
        {
            float duration = 1f;

            float jumpY = UnityEngine.Random.Range(1f, 2f);

            livingLetter.transform.DOLocalMove(startPosition, delay).OnComplete(delegate ()
            {
                livingLetter.gameObject.SetActive(true);
                livingLetter.Poof();
                livingLetter.OnJumpStart();

                float timeToJumpStart = 0.15f;
                float timeToJumpEnd = 0.4f;

                Sequence animationSequence = DOTween.Sequence();
                animationSequence.AppendInterval(timeToJumpStart);
                animationSequence.AppendCallback(delegate () { livingLetter.transform.DOLocalJump(endPosition, 7f, 1, duration).OnComplete(delegate () { if (endCallback != null) endCallback(); }).SetEase(Ease.Linear); });
                animationSequence.AppendInterval(duration - timeToJumpEnd);
                animationSequence.AppendCallback(delegate () { livingLetter.OnJumpEnded(); });
                animationSequence.AppendInterval(timeToJumpEnd);
                animationSequence.OnComplete(delegate () { livingLetter.DoHorray(); });
                animationSequence.Play();
            });
        }
    }
}
