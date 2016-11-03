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

        public void PlayIdleAnimation()
        {
            //livingLetter.SetState(LLAnimationStates.LL_normal);
            //livingLetter.SetIdle();
        }

        public void PlayWalkAnimation()
        {
            //livingLetter.SetState.State = LLAnimationStates.LL_walk;
            //livingLetter.SetWalking();
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

            livingLetter.transform.DOLocalMove(startPosition, delay).OnComplete(delegate()
            {
                livingLetter.gameObject.SetActive(true);

                livingLetter.transform.DOLocalJump(endPosition, 7f, 1, duration).OnComplete(delegate () { if (endCallback != null) endCallback(); }).SetEase(Ease.Linear);
            });
        }
    }
}
