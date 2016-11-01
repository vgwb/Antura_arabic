using UnityEngine;
using System.Collections;
using DG.Tweening;


namespace EA4S.TakeMeHome
{
	
public class TakeMeHomeTube : MonoBehaviour {

		Tweener moveTweener;

		Vector3 originalPosition;
		// Use this for initialization
		void Start () {
			originalPosition = transform.position;
		}
		
		// Update is called once per frame
		void Update () {
		
		}


		public void shake()
		{
			if (moveTweener != null)
			{
				moveTweener.Kill();
			}

			moveTweener = transform.DOShakePosition (0.5f, 0.5f,5).OnComplete(delegate () { transform.position = originalPosition; });
		}
	}
}