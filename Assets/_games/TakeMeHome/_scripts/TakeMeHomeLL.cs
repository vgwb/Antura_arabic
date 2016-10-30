using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using ArabicSupport;
namespace EA4S.TakeMeHome
{
public class TakeMeHomeLL : MonoBehaviour {

		public GameObject plane;
		public bool isDragged;

		public Transform livingLetterTransform;
		public BoxCollider boxCollider;

		public LetterObjectView letter;

		Tweener moveTweener;
		Tweener rotationTweener;

		Vector3 holdPosition;
		Vector3 normalPosition;

		private float cameraDistance;

		float maxY;

		bool dropLetter;
		bool dragging = false;
		Vector3 dragOffset = Vector3.zero;

		public event Action onMouseUpLetter;

		Action endTransformToCallback;

		public TakeMeHomeTube lastTube;

		Transform[] letterPositions;
		int currentPosition;

		void Awake()
		{
			normalPosition = transform.localPosition;
			livingLetterTransform = transform;
			holdPosition.x = normalPosition.x;
			holdPosition.y = normalPosition.y;
			lastTube = null;
		}

		public void Initialize(float _maxY, LetterObjectView _letter)
		{
			
			cameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
			letter = _letter;
			maxY = _maxY;

			dropLetter = true;
			var data = letter.Model.Data;

			TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(data, true);
		}

		public void PlayIdleAnimation()
		{
			letter.Model.State = LLAnimationStates.LL_idle_1;

			livingLetterTransform.localPosition = normalPosition;
		}

		public void PlayWalkAnimation()
		{
			letter.Model.State = LLAnimationStates.LL_walk;

			//livingLetterTransform.localPosition = normalPosition;
		}

		public void PlayHoldAnimation()
		{
			letter.Model.State = LLAnimationStates.LL_drag_idle;

			//livingLetterTransform.localPosition = holdPosition;
		}



		void MoveTo(Vector3 position, float duration)
		{
			PlayWalkAnimation();

			if (moveTweener != null)
			{
				moveTweener.Kill();
			}

			moveTweener = transform.DOLocalMove(position, duration).OnComplete(delegate () { PlayIdleAnimation(); if (endTransformToCallback != null) endTransformToCallback(); });
		}

		void RoteteTo(Vector3 rotation, float duration)
		{
			if (rotationTweener != null)
			{
				rotationTweener.Kill();
			}

			rotationTweener = transform.DORotate(rotation, duration);
		}

		void TransformTo(Transform transformTo, float duration, Action callback)
		{
			MoveTo(transformTo.localPosition, duration);
			RoteteTo(transformTo.eulerAngles, duration);

			endTransformToCallback = callback;
		}

		public void GoToFirstPostion()
		{
			GoToPosition(0);
		}

		public void GoToPosition(int positionNumber)
		{
			dropLetter = false;

			if (moveTweener != null) { moveTweener.Kill(); }
			if (rotationTweener != null) { rotationTweener.Kill(); }

			currentPosition = positionNumber;

			transform.localPosition = letterPositions[currentPosition].localPosition;
			transform.rotation = letterPositions[currentPosition].rotation;
		}

		public void MoveToNextPosition(float duration, Action callback)
		{
			dropLetter = false;

			if (moveTweener != null) { moveTweener.Kill(); }
			if (rotationTweener != null) { rotationTweener.Kill(); }

			currentPosition++;

			if (currentPosition >= letterPositions.Length)
			{
				currentPosition = 0;
			}

			TransformTo(letterPositions[currentPosition], duration, callback);
		}

		public void OnPointerDown(Vector2 pointerPosition)
		{
			if (!dragging)
			{
				lastTube = null;
				dragging = true;

				var data = letter.Model.Data;

				TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(data, true);

				Vector3 mousePosition = new Vector3(pointerPosition.x, pointerPosition.y, cameraDistance);
				Vector3 world = Camera.main.ScreenToWorldPoint(mousePosition);
				dragOffset = world - transform.position;

				OnPointerDrag(pointerPosition);

				PlayHoldAnimation();
			}
		}

		public void OnPointerDrag(Vector2 pointerPosition)
		{
			if (dragging)
			{
				dropLetter = false;

				Vector3 mousePosition = new Vector3(pointerPosition.x, pointerPosition.y, cameraDistance);

				transform.position = Camera.main.ScreenToWorldPoint(mousePosition);

				transform.position = ClampPositionToStage(transform.position - dragOffset);
			}
		}

		public void OnPointerUp()
		{
			if (dragging)
			{
				dragging = false;
				dropLetter = true;

				PlayIdleAnimation();

				if (onMouseUpLetter != null)
				{
					onMouseUpLetter();
				}


			}
		}

		void Drop(float delta)
		{
			Vector3 dropPosition = transform.position;

			dropPosition += Physics.gravity * delta;

			Debug.Log (transform.position.y);

			transform.position = ClampPositionToStage(dropPosition);
			Debug.Log (transform.position.y);
		}

		void Update()
		{
			if (dropLetter)
			{
				Drop(Time.deltaTime);
			}
		}

		Vector3 ClampPositionToStage(Vector3 unclampedPosition)
		{
			Vector3 clampedPosition = unclampedPosition;

			clampedPosition.z = -15;

			if(!isDragged)
				clampedPosition.y = clampedPosition.y < maxY ? maxY : clampedPosition.y;

			return clampedPosition;
		}

		public void EnableCollider(bool enable)
		{
			boxCollider.enabled = enable;
		}

		void OnTriggerEnter(Collider other)
		{
			TakeMeHomeTube tube = other.gameObject.GetComponent<TakeMeHomeTube> ();
			if (!tube)
				return;

			lastTube = tube;
			tube.shake ();
		}

		void OnTriggerExit(Collider other)
		{
			TakeMeHomeTube tube = other.gameObject.GetComponent<TakeMeHomeTube> ();
			if (!tube)
				return;

			lastTube = null;
		}
	}

}
