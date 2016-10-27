using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{

	public class ScannerLivingLetter : MonoBehaviour {

		private enum LLStatus { Sliding, StandingOnBelt, RunningFromAntura, Angry, Happy };

		public Animator animator;
		public GameObject livingLetter;

		public float slideSpeed = 2f;

		private LLStatus status;
		// Use this for initialization
		void Start () {
			animator.Play("LL_lose");
			status = LLStatus.Sliding;
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
			string[] animations = {"LL_idle_1", "LL_idle_2","LL_idle_3","LL_idle_4","LL_run_happy"};
			yield return new WaitForSeconds(1f);
			do
			{
				animator.Play(animations[UnityEngine.Random.Range(0, animations.Length)]);
				yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
			} while (status == LLStatus.StandingOnBelt);
		}

		void OnTriggerEnter(Collider other) 
		{
			Debug.Log("Trigger entered");
			if (status == LLStatus.Sliding)
			{
				if (other.tag == "Belt")
				{
					transform.parent = other.transform;
					status = LLStatus.StandingOnBelt;
					animator.Play("LL_land");
					StartCoroutine(RotateGO(livingLetter, new Vector3(0,180,0),1f));
					StartCoroutine(AnimateLL());
				}
			}
		}
	}
}
