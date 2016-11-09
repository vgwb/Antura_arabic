// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/08

using System;
using DG.Tweening;

namespace EA4S
{
    /// <summary>
    /// Used internally by TutorialUI to store and control animations
    /// </summary>
    public class TutorialUIAnimation
    {
        public Tween MainTween;

        internal TutorialUIAnimation(Tween _mainTween)
        {
            MainTween = _mainTween;
        }

        #region Public Methods

        public TutorialUIAnimation OnComplete(Action _callback)
        {
            if (_callback == null) MainTween.OnComplete(null);
            else MainTween.OnComplete(()=> _callback());
            return this;
        }

        #endregion
    }
}