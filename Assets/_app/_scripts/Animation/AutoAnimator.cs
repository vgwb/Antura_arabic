

using DG.Tweening;
using UnityEngine;

namespace Antura.Animation
{
    public class AutoAnimator : MonoBehaviour
    {
        public enum AnimationType
        {
            RotateZ,
            BounceLoop
        }

        #region Serialized

        public AnimationType AnimType;
        public float To;
        public float Duration = 0.35f;
        public bool ScaleInOnEnable;
        public float ScaleInDuration = 0.3f;

        #endregion

        Sequence animTween;

        #region Unity

        void OnEnable()
        {
            if (animTween != null) animTween.Restart();
            else CreateAnimation();
        }

        void OnDisable()
        {
            if (animTween != null) animTween.Pause();
        }

        void OnDestroy()
        {
            animTween.Kill();
        }

        #endregion

        #region Methods

        void CreateAnimation()
        {
            animTween = DOTween.Sequence().SetAutoKill(false);
            if (ScaleInOnEnable) {
                animTween.Append(
                    transform.DOScale(0.0001f, ScaleInDuration).From().SetEase(Ease.OutSine)
                );
            }
            switch (AnimType)
            {
                case AnimationType.RotateZ:
                    animTween = animTween.Join(
                        transform.DORotate(new Vector3(0, 0, To), Duration, RotateMode.FastBeyond360).SetLoops(int.MaxValue).SetEase(Ease.Linear)
                    );
                    break;
                case AnimationType.BounceLoop:
                    animTween = animTween.Append(
                        transform.DOScale(transform.localScale * To, Duration).SetLoops(int.MaxValue, LoopType.Yoyo).SetEase(Ease.InOutSine)
                    );
                    break;
            }
        }

        #endregion
    }
}