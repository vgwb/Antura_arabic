using UnityEngine;

namespace EA4S.Minigames.Scanner
{

	public class ScannerRotateGear : MonoBehaviour {


		public float direction;
		// Use this for initialization

		void Start () {

		}

		// Update is called once per frame
		void Update () {
			transform.Rotate(0,0,direction * ScannerConfiguration.Instance.beltSpeed);
		}
	}
}