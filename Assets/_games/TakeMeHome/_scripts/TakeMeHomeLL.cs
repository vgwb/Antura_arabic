using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
namespace EA4S.TakeMeHome
{
public class TakeMeHomeLL : MonoBehaviour {

		public GameObject plane;
		public bool isDragged;
		public bool isMoving;
        private bool isPanicing;
		public bool isDraggable;

		public Transform livingLetterTransform;
		public BoxCollider boxCollider;

		public LetterObjectView letter;

		Tweener moveTweener;
		Tweener rotationTweener;

		Vector3 holdPosition;
		Vector3 normalPosition;
        bool isResetting = false;

		private float cameraDistance;

		float maxY;

		bool dropLetter;
		bool clampPosition;
		public bool dragging = false;
		Vector3 dragOffset = Vector3.zero;
		Vector3 tubeSpawnPosition;
		public bool respawn;

		public event Action onMouseUpLetter;

		Action endTransformToCallback;
        

        public List<TakeMeHomeTube> collidedTubes;

		void Awake()
		{
			normalPosition = transform.localPosition;
			livingLetterTransform = transform;
			holdPosition.x = normalPosition.x;
			isMoving = false;
			isDraggable = false;
			holdPosition.y = normalPosition.y;
			//lastTube = null;
			respawn = true;
            isResetting = false;
            isPanicing = false;
            collidedTubes = new List<TakeMeHomeTube>();
        }

		public void Initialize(float _maxY, LetterObjectView _letter, Vector3 tubePosition)
		{
			tubeSpawnPosition = tubePosition;

			cameraDistance =  (transform.position.z) - Camera.main.transform.position.z;

			//cameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
			letter = _letter;
			maxY = _maxY;

			dropLetter = false;
            isResetting = false;
            clampPosition = false;

		}

		public void PlayIdleAnimation()
        {
            letter.SetState(LLAnimationStates.LL_idle);

			//livingLetterTransform.localPosition = normalPosition;
		}

		public void PlayWalkAnimation()
		{
			letter.SetState(LLAnimationStates.LL_walking);
            letter.SetWalkingSpeed(LetterObjectView.WALKING_SPEED);

			//livingLetterTransform.localPosition = normalPosition;
		}

		public void PlayHoldAnimation()
        {
            letter.SetState(LLAnimationStates.LL_dragging);

            //livingLetterTransform.localPosition = holdPosition;
        }



		public void MoveTo(Vector3 position, float duration)
		{
			isMoving = true;
			PlayWalkAnimation();

			transform.rotation = Quaternion.Euler (new Vector3 (0, -90, 0));

			if (moveTweener != null)
			{
				moveTweener.Kill();
			}

			moveTweener = transform.DOLocalMove(position, duration).OnComplete(delegate () { 
				PlayIdleAnimation(); 
				if (endTransformToCallback != null) endTransformToCallback();

				//play audio
				//TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letter.Data, true);
				RotateTo(new Vector3 (0, 180, 0),0.5f);
				isMoving = false;
			});
		}

        public void sayLetter()
        {
            TakeMeHomeConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letter.Data, true);
        }

		public void MoveBy(Vector3 position, float duration)
		{
			MoveTo (transform.position + position, duration);
		}


		void RotateTo(Vector3 rotation, float duration)
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
			RotateTo(transformTo.eulerAngles, duration);

			endTransformToCallback = callback;
		}

		

		

		

		public void OnPointerDown(Vector2 pointerPosition)
		{
			if (isMoving || !isDraggable)
				return;
			
			if (!dragging)
			{
               // if (lastTube) lastTube.deactivate();
                //lastTube = null;
				dragging = true;

				var data = letter.Data;

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

                
                //check if position should clamp:
                if (transform.position.x > 5.4f)// && transform.position.y > maxY)
					clampPosition = true;

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


			if(clampPosition) transform.position = ClampPositionToStage(dropPosition);
			else transform.position = dropPosition;//ClampPositionToStage(dropPosition);

			//free fall:
			if (!clampPosition) {
				if (respawn && transform.position.y < (maxY - 20)) {
					AudioManager.I.PlaySfx (Sfx.Splat);
                    //transform.position = 
                    isPanicing = false;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    transform.position = tubeSpawnPosition;
					clampPosition = true;
                    transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    transform.DOScale(0.8f, 0.5f);

                }
            }
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

			

			if(!dragging)
				clampedPosition.y = clampedPosition.y < maxY ? maxY : clampedPosition.y;

			if (clampedPosition.y == maxY) {
				dropLetter = false;
				clampPosition = false;
                isResetting = false;

            }
			
			return clampedPosition;
		}

		private void moveUp()
		{
            if (collidedTubes.Count == 0) return;
			//if (lastTube == null)
			//	return;
			
			if (moveTweener != null)
			{
				moveTweener.Kill();
			}
            isResetting = true;
            transform.DOScale(0.1f, 0.1f);

            Vector3 targetPosition = collidedTubes[collidedTubes.Count-1].transform.FindChild("Cube").position; ;// lastTube.transform.FindChild("Cube").position;
            collidedTubes[collidedTubes.Count - 1].deactivate();
          

            moveTweener = transform.DOLocalMove(targetPosition/*transform.position + lastTube.transform.up*5 + new Vector3(0,0,20)*/, 0.3f).OnComplete(delegate () { 
				PlayIdleAnimation(); 
				if (endTransformToCallback != null) endTransformToCallback();

                /*if(lastTube != null)
                {
                    lastTube.hideWinParticles();
                }*/

                
                //lastTube = null;

                foreach(TakeMeHomeTube tube in collidedTubes)
                {
                    tube.deactivate();
                    tube.hideWinParticles();
                }
                collidedTubes.Clear();

                    transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				transform.position = tubeSpawnPosition;
                transform.DOScale(0.8f, 0.5f);
                clampPosition = true;
				dropLetter = true;
				isMoving = false;

            });
		}

		public void panicAndRun()
		{
            //wait few milliseconds then move:
            if (isPanicing) return;
            isPanicing = true;
            isMoving = true;
			isDraggable = false;
			dropLetter = false;

           // StartCoroutine(waitForSeconds(1, ()=> {

                RotateTo(new Vector3(0, -90, 0), 0.5f);


                letter.SetState(LLAnimationStates.LL_walking);
                letter.SetWalkingSpeed(LetterObjectView.RUN_SPEED);

                if (moveTweener != null)
                {
                    moveTweener.Kill();
                }

                moveTweener = transform.DOLocalMove(new Vector3(5.2f, -3.44f, -15), 0.5f).OnComplete(delegate () {
                  
                    PlayIdleAnimation();
                    respawn = false;
                    clampPosition = false;



                    dropLetter = true;

                    isMoving = false;
                });

          //  }));

            

		}


		public void followTube(bool win)
		{

            if (isResetting) return;

            Debug.Log("following tube");
            isResetting = true;
			isMoving = true;
			isDraggable = false;
			dropLetter = false;


            if(win)
            {
                //lastTube.showWinParticles();
                if(collidedTubes.Count > 1)
                {
                    collidedTubes[collidedTubes.Count - 1].showWinParticles();
                }
            }




		
			moveUp ();
		


		}

		IEnumerator waitForSeconds(float seconds, Action action)
		{
			yield return new WaitForSeconds (seconds);
            action();

		}

		IEnumerator waitForSecondsAndJump(float seconds)
		{
			yield return new WaitForSeconds (seconds);
			letter.SetState(LLAnimationStates.LL_walking);
			letter.SetWalkingSpeed(LetterObjectView.RUN_SPEED);

			if (moveTweener != null)
			{
				moveTweener.Kill();
			}

			moveTweener = transform.DOLocalMove(transform.position - (new Vector3(5,0,0)), 1).OnComplete(delegate () { 
				
				clampPosition = false;
                transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                transform.DOScale(0.8f, 0.5f);
                dropLetter = true;
				isMoving = false;
			});
		}


		public void EnableCollider(bool enable)
		{
			boxCollider.enabled = enable;
		}

		void OnTriggerEnter(Collider other)
		{
            if (isResetting) return;

            TakeMeHomeTube tube = other.gameObject.GetComponent<TakeMeHomeTube>();
            if (!tube || collidedTubes.IndexOf(tube) != -1)
                return;

            if (collidedTubes.Count > 0)
                collidedTubes[collidedTubes.Count - 1].deactivate();

            collidedTubes.Add(tube);
            tube.activate();

            /*
                        if (!dragging) {
                            if (lastTube) lastTube.deactivate();
                            lastTube = null;
                            return;
                        }

                        TakeMeHomeTube tube = other.gameObject.GetComponent<TakeMeHomeTube> ();
                        if (!tube)
                            return;
                        Debug.Log("entering tube: " + tube.gameObject.name);
                        if (lastTube) lastTube.deactivate();
                        lastTube = tube;


                        tube.shake ();
                        tube.activate();*/
        }

        


        void OnTriggerExit(Collider other)
		{
            TakeMeHomeTube tube = other.gameObject.GetComponent<TakeMeHomeTube>();
            if (!tube) return;
            popTube(tube);

            /*
                        Debug.Log("exiting: " + other.gameObject.name);
                       // if (!isDraggable) return;

                        TakeMeHomeTube tube = other.gameObject.GetComponent<TakeMeHomeTube> ();
                        if(tube)
                        {
                            tube.deactivate();
                            Debug.Log("exiting tube: " + tube.gameObject.name);
                        }



                        if (!tube || lastTube != tube)
                            return;
                        if (lastTube) lastTube.deactivate();
                        lastTube = null;*/
        }

        void popTube(TakeMeHomeTube tube)
        {
            tube.deactivate();
            collidedTubes.Remove(tube);

            if (collidedTubes.Count > 0)
                collidedTubes[collidedTubes.Count - 1].activate();
        }
	}

}
