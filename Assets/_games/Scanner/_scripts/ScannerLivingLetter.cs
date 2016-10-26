using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{

	public class ScannerLivingLetter : MonoBehaviour {

		public Animator animator;

		public float slideSpeed = 0.2f;

		// Use this for initialization
		void Start () {
			animator.Play("LL_lose");
		}

		// Update is called once per frame
		void Update () {
			transform.Translate(slideSpeed, -slideSpeed/2,0);
		}
	}
}
