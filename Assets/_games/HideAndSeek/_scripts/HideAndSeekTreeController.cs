using UnityEngine;
using System.Collections;

namespace EA4S.HideAndSeek
{
public class HideAndSeekTreeController : MonoBehaviour {

	// Use this for initialization
		public delegate void TouchAction(int i);
		public static event TouchAction onTreeTouched;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown()
		{
			if (onTreeTouched != null) {
				//Debug.Log ("Send Event");
				onTreeTouched (id);
			}

		}
		//var
		public int id;
}

}
