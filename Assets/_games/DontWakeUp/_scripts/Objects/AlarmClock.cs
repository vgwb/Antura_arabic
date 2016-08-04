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
        public GameObject DangerLine;

        Tween hoursTween, minutesTween;

        void Start()
        {
            originalPosition = transform.position;
            hoursTween = HoursGO.transform.DORotate(new Vector3(0, 0, 360 * (Random.Range(0, 100) > 50 ? 1 : -1)), Random.Range(15, 25), RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1).Play();
            minutesTween = MinutesGO.transform.DORotate(new Vector3(0, 0, 360 * (Random.Range(0, 100) > 50 ? 1 : -1)), Random.Range(4, 6), RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1).Play();
            HideDangerLine();
        }

        void OnDestroy()
        {
            hoursTween.Kill();
            minutesTween.Kill();
        }

        void AlarmOn()
        {
            Debug.Log("AlarmClock.AlarmOn()");
            transform.DOShakePosition(5).Play();
            AudioManager.I.PlaySfx(Sfx.AlarmClock);

            HideDangerLine();
        }

        void AlarmOff()
        {
            Debug.Log("AlarmClock.AlarmOff()");
            AudioManager.I.StopSfx(Sfx.AlarmClock);
            transform.position = originalPosition;
        }

        void OnMouseDown()
        {
            // Debug.Log("OnMouseDown on AlarmClock");
        }

             
        void OnEnable()
        {
            HideDangerLine();
        }

        void OnDisable()
        {
            HideDangerLine();
        }

        public void HideDangerLine()
        {
            DangerLine.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                DangerLine.SetActive(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player") {
                HideDangerLine();
            }

        }

    }
}