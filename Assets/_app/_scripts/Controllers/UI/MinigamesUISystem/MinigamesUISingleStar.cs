// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/29

using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class MinigamesUISingleStar : MonoBehaviour
    {
        public Transform Star;

        bool hasStar;
        Tween loseTween, gainTween;

        #region Unity

        void Awake()
        {
            // Tweens
            loseTween = Star.DOScale(0.001f, 0.25f).SetAutoKill(false).Pause()
                .OnComplete(()=> Star.gameObject.SetActive(false));
            loseTween.ForceInit();
            gainTween = Star.DOScale(0.001f, 0.5f).From().SetEase(Ease.OutElastic, 1.70f, 0.5f).SetAutoKill(false).Pause();

            loseTween.Complete();
        }

        void OnDestroy()
        {
            loseTween.Kill();
            gainTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Gain()
        {
            if (hasStar) return;

            hasStar = true;
            loseTween.Rewind();
            Star.gameObject.SetActive(true);
            gainTween.Restart();
        }

        public void Lose()
        {
            if (!hasStar) return;

            hasStar = false;
            gainTween.Complete();
            Star.gameObject.SetActive(true);
            loseTween.Restart();
        }

        #endregion
    }
}