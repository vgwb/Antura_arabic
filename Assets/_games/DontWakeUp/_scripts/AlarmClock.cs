using UnityEngine;
using System.Collections;

public class AlarmClock : MonoBehaviour
{

    // Use this for initialization
    void Start() {
	
    }
	
    // Update is called once per frame
    void Update() {
	
    }

    void OnMouseDown() {
        Debug.Log("OnMouseDown o nAlarmClock");
        EA4S.DontWakeUp.GameDontWakeUp.Instance.ChangeCamera();
    }
}
