using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S
{
    public class AlarmClock : MonoBehaviour
    {

        Vector3 originalPosition;

        void Start() {
            originalPosition = transform.position;
        }

        void AlarmOn() {
            Debug.Log("AlarmClock.AlarmOn()");
            transform.DOShakePosition(3).Play();
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