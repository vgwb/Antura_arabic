using UnityEngine;
using System.Collections;

namespace EA4S.DancingDots
{
	public class DancingDotsSplat : MonoBehaviour {

		// Use this for initialization
		void Start () {
			StartCoroutine(AnimateSplat(transform));

		}

		// Update is called once per frame
		void Update () {

		}

		public float splatMaxSize = 5.0f;
		public float splatMaxY = -22;

		public float splatGrowFactor = 5f;
		public float splatSlideFactor = 10f;
		public float splatWaitTime = 1f;

		IEnumerator AnimateSplat(Transform trans)
		{


			float timer = 0;

			AudioManager.I.PlaySfx(Sfx.Splat);

			// Scale
			while(splatMaxSize > trans.localScale.x)
			{
				timer += Time.deltaTime;
				trans.localScale += Vector3.one * Time.deltaTime * splatGrowFactor;
				yield return null;
			}


			yield return new WaitForSeconds(splatWaitTime);

			//Slide
			while(splatMaxY < trans.position.y)
			{
				timer += Time.deltaTime;
				trans.position -= new Vector3(0, Time.deltaTime * splatSlideFactor, 0);
				yield return null;
			}

			Destroy(trans.gameObject);

		}
	}
}
