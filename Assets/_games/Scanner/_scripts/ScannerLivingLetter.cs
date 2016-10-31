using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{

	public class ScannerLivingLetter : MonoBehaviour {

		private enum LLStatus { Sliding, StandingOnBelt, RunningFromAntura, Angry, Happy };

		public Animator animator;
		public GameObject livingLetter;

		public float slideSpeed = 2f;

        /*

		private LLStatus status;
		// Use this for initialization
		void Start () {
			livingLetter.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_lose);
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

			yield return new WaitForSeconds(1f);

			int index = -1;
			LLAnimationStates[] animations = 
			{
				LLAnimationStates.LL_idle_1,
				LLAnimationStates.LL_idle_2,
				LLAnimationStates.LL_idle_3,
				LLAnimationStates.LL_idle_4,
				LLAnimationStates.LL_dancing_1, 
				LLAnimationStates.LL_run_happy,
				LLAnimationStates.LL_turn_180,
				LLAnimationStates.LL_twirl
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
			Debug.Log("Trigger entered");
			if (status == LLStatus.Sliding)
			{
				if (other.tag == "Scanner_Belt")
				{
					transform.parent = other.transform;
					status = LLStatus.StandingOnBelt;
					livingLetter.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_land);
					StartCoroutine(RotateGO(livingLetter, new Vector3(0,180,0),1f));
					StartCoroutine(AnimateLL());
				}
			}
		}

    */
	}
}
