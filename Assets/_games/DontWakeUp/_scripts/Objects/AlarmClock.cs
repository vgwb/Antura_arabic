using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S
{
    public class AlarmClock : MonoBehaviour
    {

        Vector3 originalPosition;
        public GameObject HoursGO;
        public GameObject MinutesGO;

        Tween hoursTween, minutesTween;

        void Start() {
            originalPosition = transform.position;
            hoursTween = HoursGO.transform.DORotate(new Vector3(0, 0, 360), 20, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1).Play();
            minutesTween = MinutesGO.transform.DORotate(new Vector3(0, 0, 360), 5, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1).Play();
        }

        void OnDestroy() {
            hoursTween.Kill();
            minutesTween.Kill();
        }

        void AlarmOn() {
            Debug.Log("AlarmClock.AlarmOn()");
            transform.DOShakePosition(5).Play();
            AudioManager.I.PlaySfx(Sfx.AlarmClock);
        }

        void AlarmOff() {
            Debug.Log("AlarmClock.AlarmOff()");
            AudioManager.I.StopSfx(Sfx.AlarmClock);
            transform.position = originalPosition;
        }

        void OnMouseDown() {
            Debug.Log("OnMouseDown on AlarmClock");
        }
    }
}