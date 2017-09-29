

using DG.Tweening;
using UnityEngine;

namespace Antura.Animation
{
    public class AutoAnimator : MonoBehaviour
    {
        public enum AnimationType
        {
            RotateZ
        }

        #region Serialized

        public AnimationType AnimType;
        public float To;
        public float Duration;

        #endregion

        Tween animTween;

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
            switch (AnimType)
            {
                case AnimationType.RotateZ:
                    animTween = transform.DORotate(new Vector3(0, 0, To), Duration, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
                    break;
            }
        }

        #endregion
    }
}