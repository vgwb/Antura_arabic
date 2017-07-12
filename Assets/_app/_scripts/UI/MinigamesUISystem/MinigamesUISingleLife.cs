using DG.Tweening;
using UnityEngine;

namespace Antura.UI
{
    /// <summary>
    /// A single life icon for the MinigamesUILives script.
    /// </summary>
    public class MinigamesUISingleLife : MonoBehaviour
    {
        public Transform Heart;

        bool hasHeart = true;
        Tween loseTween, gainTween;

        #region Unity

        void Awake()
        {
            // Tweens
            loseTween = Heart.DOScale(0.001f, 0.25f).SetAutoKill(false).Pause()
                .OnComplete(() => Heart.gameObject.SetActive(false));
            gainTween = Heart.DOScale(0.001f, 0.5f).From().SetEase(Ease.OutElastic, 1.70f, 0.5f).SetAutoKill(false);
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
            loseTween.Rewind();
            Heart.gameObject.SetActive(true);
            if (!hasHeart && !gainTween.IsPlaying()) gainTween.Restart();
            hasHeart = true;
        }

        public void Lose()
        {
            gainTween.Complete();
            if (hasHeart && !loseTween.IsPlaying()) {
                Heart.gameObject.SetActive(true);
                loseTween.Restart();
                hasHeart = false;
            }
        }

        #endregion
    }
}