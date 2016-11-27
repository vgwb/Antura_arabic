using UnityEngine;
using System.Collections;


namespace EA4S.Scanner
{
	public class ScannerAntura : MonoBehaviour {

		private AnturaAnimationController antura;

		// Use this for initialization
		void Start () {
			antura = GetComponent<AnturaAnimationController>();
//			antura.WalkingSpeed = 0;
			antura.SetWalkingSpeed(AnturaAnimationController.WALKING_SPEED);
			antura.State = AnturaAnimationStates.sucking;
		}

		// Update is called once per frame
		void Update () {
		}
	}
}