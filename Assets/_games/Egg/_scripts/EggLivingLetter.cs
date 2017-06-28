using System;
using UnityEngine;
using DG.Tweening;
using EA4S.LivingLetters;
using EA4S.MinigamesAPI;

namespace EA4S.Minigames.Egg
{
    public class EggLivingLetter
    {
        LivingLetterController livingLetter;
        Transform shadowTransform;

        Vector3 startPosition;
        Vector3 endPosition;

        float startScale = 0.4f;
        float endScale = 1f;

        Vector3 shadowLocalPosition;

        float delay;

        Action endCallback;

        public EggLivingLetter(Transform parent, GameObject letterObjectViewPrefab, GameObject shadowPrefab, ILivingLetterData livingLetterData, Vector3 startPosition, Vector3 shadowPosition, Vector3 endPosition, float delay, Action endCallback)
        {
            livingLetter = UnityEngine.Object.Instantiate(letterObjectViewPrefab).GetComponent<LivingLetterController>();

            livingLetter.transform.SetParent(parent);
            livingLetter.transform.localPosition = startPosition;
            livingLetter.Initialize(livingLetterData);
            livingLetter.transform.localScale *= startScale;
            livingLetter.gameObject.SetActive(false);

            shadowTransform = UnityEngine.Object.Instantiate(shadowPrefab).transform;
            shadowTransform.SetParent(parent);
            shadowLocalPosition = shadowPosition;
            shadowTransform.localPosition = shadowLocalPosition;
            shadowTransform.localScale *= 0.7f;
            shadowTransform.gameObject.SetActive(false);

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

        public void Update(float delta)
        {
            shadowLocalPosition.x = livingLetter.transform.localPosition.x;
            shadowLocalPosition.z = livingLetter.transform.localPosition.z;

            shadowTransform.localPosition = shadowLocalPosition;
        }

        public void DestroyLetter()
        {
            UnityEngine.Object.Destroy(livingLetter.gameObject);
            UnityEngine.Object.Destroy(shadowTransform.gameObject);
        }

        public void JumpToEnd()
        {
            float duration = 1f;

            livingLetter.transform.DOLocalMove(startPosition, delay).OnComplete(delegate ()
            {
                shadowTransform.gameObject.SetActive(true);
                livingLetter.gameObject.SetActive(true);

                livingLetter.transform.DOScale(endScale, duration * 0.5f);

                //livingLetter.Poof();
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
