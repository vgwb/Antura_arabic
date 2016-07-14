using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S
{
    public class AlarmClock : MonoBehaviour
    {

        // Use this for initialization
        void AlarmOn() {
            transform.DOShakePosition(3).Play();
            AudioManager.I.PlaySfx(Sfx.AlarmClock);
        }
	
        // Update is called once per frame
        void Update() {
	
        }

        void OnMouseDown() {
            Debug.Log("OnMouseDown o nAlarmClock");
            //EA4S.DontWakeUp.GameDontWakeUp.Instance.ChangeCamera();
        }
    }
}