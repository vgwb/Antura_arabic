using UnityEngine;
using System.Collections;


namespace EA4S.Scanner
{
	public class ScannerBelt : MonoBehaviour {

		public float speed = 0.2f;
		public Transform spawnPoint;
		public Transform endPoint;
		
		void FixedUpdate()
		{
			transform.Translate(speed,0,0);
			if (speed > 0 && transform.position.x > endPoint.position.x ||
				speed < 0 && transform.position.x < endPoint.position.x )
			{
				transform.position = spawnPoint.position;
			}
		}
	}
}
