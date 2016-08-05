using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S
{
    public class Plant : MonoBehaviour
    {
        public float MinRotationOffset = -24;
        public float MaxRotationOffset = 24;
        public float MinDuration = 1;
        public float MaxDuration = 3;

        Tween myTween;

        void Start()
        {
            float rotDiff = UnityEngine.Random.Range(MinRotationOffset, MaxRotationOffset);
            Vector3 rot = transform.rotation.eulerAngles;
            float toX = rot.x + rotDiff;
            rot.x -= rotDiff;

            myTween = transform.DORotate(new Vector3(toX, rot.y, rot.z), UnityEngine.Random.Range(MinDuration, MaxDuration), RotateMode.FastBeyond360)
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        void OnDestroy()
        {
            myTween.Kill();
        }

    }
}