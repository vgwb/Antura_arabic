using UnityEngine;
using System.Collections;


namespace EA4S.Scanner
{
	public class ScannerBelt : MonoBehaviour {

		public float speed = 0.2f;
		public Transform spawnPoint;
		public Transform endPoint;

		void Start()
		{
			if (speed > 0)
			{
				foreach(Transform beltPiece in this.transform)
				{
					beltPiece.tag = "Scanner_Belt";
				}
			}
		}

		void Update()
		{
			foreach (Transform beltPiece in this.transform)
			{
				beltPiece.Translate(speed * Time.deltaTime,0,0);
				if (speed > 0 && beltPiece.position.x > endPoint.position.x ||
					speed < 0 && beltPiece.position.x < endPoint.position.x )
				{
					beltPiece.position = spawnPoint.position;
				}
			}
		}
	}
}
