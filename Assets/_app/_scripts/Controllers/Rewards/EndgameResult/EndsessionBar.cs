// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/21

using DG.Tweening;
using UnityEngine;

namespace EA4S
{
    public class EndsessionBar : MonoBehaviour
    {
        public RectTransform Bar;
        public EndsessionAchievement[] Achievements;

        Tween showTween;

        #region Unity

        void Awake()
        {
            showTween = this.GetComponent<RectTransform>().DOAnchorPosX(-1300, 0.35f).From().SetAutoKill(false).Pause();
        }

        void OnDestroy()
        {
            showTween.Kill();
        }

        #endregion

        #region Public Methods

        internal void Show()
        {
            showTween.Restart();
        }

        internal void Hide()
        {
            showTween.Rewind();
        }

        #endregion
    }
}