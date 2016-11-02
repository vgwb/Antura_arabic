// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/01

using DG.DeExtensions;
using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TutorialUIProp : MonoBehaviour
    {
        public bool IsPooled;

        SpriteRenderer img;
        Transform defParent;
        Vector3 lastPos;
        Tween showTween;

        #region Unity

        void Awake()
        {
            defParent = this.transform.parent;
            img = this.GetComponentInChildren<SpriteRenderer>(true);

            img.SetAlpha(0);
            showTween = img.DOFade(1, 0.2f).SetAutoKill(false).Pause()
                .SetEase(Ease.Linear)
                .OnRewind(() => {
                    this.gameObject.SetActive(false);
                    this.transform.parent = defParent;
                });

            if (!IsPooled) this.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            showTween.Kill();
        }

        #endregion

        #region Public Methods

        public void Show(Transform _parent, Vector3? _position)
        {
            this.transform.parent = _parent;
            if (_position != null) this.transform.position = (Vector3)_position;
            this.transform.localScale = Vector3.one * (TutorialUI.I.Cam.fieldOfView / 45f);
            this.gameObject.SetActive(true);
            showTween.PlayForward();
        }

        public void Hide(bool _immediate = false)
        {
            if (_immediate) showTween.Rewind();
            else showTween.PlayBackwards();
        }

        #endregion
    }
}