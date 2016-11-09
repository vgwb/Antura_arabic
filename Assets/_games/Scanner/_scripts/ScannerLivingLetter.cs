using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{

	public class ScannerLivingLetter : MonoBehaviour {

		private enum LLStatus { None, Sliding, StandingOnBelt, RunningFromAntura, Angry, Happy };
		public GameObject livingLetter;
		public float slideSpeed = 2f;
		public bool facingCamera;
		private LLStatus status = LLStatus.None;
		private float turnAngle;
		private Vector3 startingPosition;
		private Quaternion startingRotation;

		// Use this for initialization
		void Start () 
		{
			startingPosition = transform.position;
			startingRotation = livingLetter.GetComponent<LetterObjectView>().transform.rotation;

//			Reset();

		}


		public void Reset()
		{
			StopAllCoroutines();
			transform.position = startingPosition;
			livingLetter.GetComponent<LetterObjectView>().transform.rotation = startingRotation;

			turnAngle = facingCamera ? 180 : 0;
			livingLetter.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_still);
			livingLetter.GetComponent<LetterObjectView>().Falling = true;
			status = LLStatus.Sliding;
			gameObject.GetComponent<SphereCollider>().enabled = true; // enable feet collider
			gameObject.GetComponent<BoxCollider>().enabled = false; // disable body collider

		}

		// Update is called once per frame
		void Update () {
			if (status == LLStatus.Sliding)
			{
				transform.Translate(slideSpeed * Time.deltaTime, -slideSpeed * Time.deltaTime / 2,0);
			}
		}

		IEnumerator RotateGO(GameObject go, Vector3 toAngle, float inTime) {
			var fromAngle = go.transform.rotation;
			var destAngle = Quaternion.Euler(toAngle);
			for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
				go.transform.rotation = Quaternion.Lerp(fromAngle, destAngle, t);
				yield return null;
			}
		}

		IEnumerator AnimateLL()
		{

			yield return new WaitForSeconds(1f);

			int index = -1;
			LLAnimationStates[] animations = 
			{
				LLAnimationStates.LL_idle,
				LLAnimationStates.LL_dancing, 
				//LLAnimationStates.LL_walking
			};

			do
			{
				int oldIndex = index;
				do
				{
					index = UnityEngine.Random.Range(0, animations.Length);
				} while (index == oldIndex);
				livingLetter.GetComponent<LetterObjectView>().SetState(animations[index]);
				yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));
			} while (status == LLStatus.StandingOnBelt);
		}

		void OnTriggerEnter(Collider other) 
		{
			if (status == LLStatus.Sliding)
			{
				if (other.tag == ScannerGame.TAG_BELT)
				{
					transform.parent = other.transform;
					status = LLStatus.StandingOnBelt;
					gameObject.GetComponent<SphereCollider>().enabled = false; // disable feet collider
					gameObject.GetComponent<BoxCollider>().enabled = true; // enable body collider
					livingLetter.GetComponent<LetterObjectView>().Falling = false;
					StartCoroutine(RotateGO(livingLetter, new Vector3(0,turnAngle,0),1f));
					StartCoroutine(AnimateLL());
				}
			}
		}
        
	}
}
