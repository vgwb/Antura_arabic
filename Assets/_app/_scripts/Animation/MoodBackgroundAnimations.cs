// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/28 11:50
// License Copyright (c) Daniele Giardini

using DG.Tweening;
using UnityEngine;

namespace EA4S.Animation
{
    public class MoodBackgroundAnimations : MonoBehaviour
    {
        [Header("Options")]
        public float MinRotationOffset = 5;
        public float MaxRotationOffset = 24;
        public float MinDuration = 4;
        public float MaxDuration = 8;
        [Header("References")]
        public RectTransform[] Ferns;

        Tween[] fernTweens;

        void Start()
        {
            fernTweens = new Tween[Ferns.Length];
            for (int i = 0; i < Ferns.Length; ++i) {
                RectTransform rt = Ferns[i];
                float rotDiff = UnityEngine.Random.Range(MinRotationOffset, MaxRotationOffset);
                Vector3 rot = rt.eulerAngles;
                float toZ = rot.z + rotDiff;
                rot.z -= rotDiff;
                fernTweens[i] = Ferns[i].DORotate(new Vector3(0, 0, toZ), UnityEngine.Random.Range(MinDuration, MaxDuration), RotateMode.FastBeyond360)
                    .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        void OnDestroy()
        {
            foreach (Tween t in fernTweens) t.Kill();
        }
    }
}