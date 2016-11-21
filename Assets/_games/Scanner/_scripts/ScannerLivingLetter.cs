using UnityEngine;
using System.Collections;
using System;

namespace EA4S.Scanner
{

	public class ScannerLivingLetter : MonoBehaviour {

		public enum LLStatus { None, Sliding, StandingOnBelt, RunningFromAntura, Lost, Won, Happy, Sad };
		public GameObject livingLetter;
		public float slideSpeed = 2f;
		public bool facingCamera;
		public LLStatus status = LLStatus.None;
		private float turnAngle;
		private Vector3 startingPosition;
		private Quaternion startingRotation;

		public LetterObjectView letterObjectView;

		public event Action onReset;


		void Start()
		{
			letterObjectView = livingLetter.GetComponent<LetterObjectView>();
			startingPosition = transform.position;
			startingRotation = letterObjectView.transform.rotation;

			Reset();
		}

		public void Reset()
		{
			StopAllCoroutines();

			letterObjectView.transform.rotation = startingRotation;
			transform.position = startingPosition;

			turnAngle = facingCamera ? 180 : 0;
			letterObjectView.SetState(LLAnimationStates.LL_still);
			letterObjectView.Falling = true;
			status = LLStatus.Sliding;
			gameObject.GetComponent<SphereCollider>().enabled = true; // enable feet collider
			gameObject.GetComponent<BoxCollider>().enabled = false; // disable body collider
			onReset();

		}

		// Update is called once per frame
		void Update () {
			if (status == LLStatus.Sliding)
			{
				transform.Translate(slideSpeed * Time.deltaTime, -slideSpeed * Time.deltaTime / 2,0);
			}
			else if (status == LLStatus.Won)
			{
				// TODO fly then Reset
				Reset();
			}
			else if (status == LLStatus.Lost)
			{
				// Poof then Reset
				Reset();
			}
		}


		void OnMouseUp()
		{
			letterObjectView.SetState(LLAnimationStates.LL_tickling);
		}

		public void RoundLost()
		{
			StopAllCoroutines();
			letterObjectView.SetState(LLAnimationStates.LL_idle);
			status = LLStatus.Lost;
		}

		public void RoundWon()
		{

			status = LLStatus.Won;
		}

		public void CorrectMove()
		{
			StopAllCoroutines();
//			letterObjectView.SetState(LLAnimationStates.LL_idle);
			letterObjectView.DoHorray();
		}

		public void WrongMove()
		{
			StopAllCoroutines();
			letterObjectView.SetState(LLAnimationStates.LL_idle);
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
				letterObjectView.SetState(animations[index]);
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
					letterObjectView.Falling = false;
					StartCoroutine(RotateGO(livingLetter, new Vector3(0,turnAngle,0),1f));
					StartCoroutine(AnimateLL());
				}
			}
		}
        
	}
}
