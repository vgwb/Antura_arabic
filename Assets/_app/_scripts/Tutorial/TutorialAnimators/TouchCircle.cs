// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/04 00:40
// License Copyright (c) Daniele Giardini

using DG.Tweening;
using UnityEngine;

namespace EA4S.TutorialAnimators
{
    public class TouchCircle : MonoBehaviour
    {
        [Header("Options")]
        public float ScaleMultiplier = 1.2f;
        public float PulseDuration = 0.5f;

        Tween pulseTween;

        void Start()
        {
            pulseTween = this.transform.DOScale(this.transform.localScale * ScaleMultiplier, PulseDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        }

        void OnDestroy()
        {
            pulseTween.Kill();
        }
    }
}