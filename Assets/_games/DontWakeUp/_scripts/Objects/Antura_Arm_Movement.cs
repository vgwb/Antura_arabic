using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Antura_Arm_Movement : MonoBehaviour
{

    public float Delay;
    public float Duration;
    public Vector3 DestinationAngle;
    public Vector3 DestinationPosition;
    public bool FastBeyod360;

    void OnEnable() {
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(Delay)
            .Append(transform.DOLocalRotate(DestinationAngle, Duration, (FastBeyod360 ? RotateMode.FastBeyond360 : RotateMode.Fast)))
            .Join(transform.DOLocalMove(DestinationPosition, Duration))
            .SetRelative(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

        //DOLocalRotate(Vector3 to, float duration, RotateMode mode);

        // DOTween.To(() => myValue, x => myValue = x, 0, 3).SetOptions(true).SetDelay(1);

        /*                DOTween.Sequence()
                                   .Append(transform.DOMoveX(_closedX, 0.2f, false).SetEase(Ease.Linear))
                                   .AppendCallback(ChangeTextDirect)
                                   .Append(transform.DOMoveX(0.0f, 0.5f, false).SetEase(Ease.Linear));*/


    }

}
