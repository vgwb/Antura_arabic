using UnityEngine;
using System.Collections;
using DG.Tweening;
using ArabicSupport;
using TMPro;
using EA4S.Db;

namespace EA4S.HideAndSeek
{
    public enum MovementType
    {
        Normal,
        OnlyRight,
        OnlyLeft,
        Enhanced
    }

    public class HideAndSeekLetterController : MonoBehaviour {
        
		public delegate void TouchAction(int i);
		public static event TouchAction onLetterTouched;
	
	    void Start ()
        {
            view = GetComponent<LetterObjectView>();
	    }

        public void resultAnimation(bool win)
        {
            if (moveTweener != null)
            {
                moveTweener.Kill();
            }

            if(win)
            {
                view.SetState(LLAnimationStates.LL_dancing);
                view.DoDancingWin();
            }
            else
            {
                view.SetState(LLAnimationStates.LL_dancing);
                view.DoDancingLose();
            }
        }
        
        void MoveTo(Vector3 position, float duration)
        {
            view.SetState(LLAnimationStates.LL_walking);
            if (position == pos1)
                view.HasFear = true;
            
            view.SetWalkingSpeed(1f);
            if (moveTweener != null)
            {
                moveTweener.Kill();
            }

            moveTweener = transform.DOLocalMove(position, duration).OnComplete(
                delegate () {
                    if (position == pos2)
                    {
                        startTime = Time.time;
                        isArrived = true;
                    } else if(position == pos1)
                    {
                        isMoving = false;
                        isClickable = false;
                        view.SetState(LLAnimationStates.LL_idle);
                        view.HasFear = false;
                    } else
                    {
                        view.SetState(LLAnimationStates.LL_idle);
                    }
                });
        }

        public void ResetLetter()
        {
            GetComponent<Animator>().SetTrigger("doReset");
            view.SetState(LLAnimationStates.LL_idle);
            isArrived = false;
            isMoving = false;
            isClickable = false;
        }

        void Update ()
        {
            if(isArrived && Time.time > startTime + idleTime )
            {
                MoveTo(pos1, walkDuration / 2);
                isArrived = false;
            }
		}

        public void SetStartPosition(Vector3 pos)
        {
            pos1 = pos;
        }

        public void MoveTutorial()
        {
            Vector3 pos = new Vector3(transform.position.x - 4.0f, transform.position.y, transform.position.z);
            isClickable = true;
            MoveTo(pos, walkDuration);
        }


        public void Move()
        {
			if (!isMoving) {
                float temp = Random.Range(-ray, ray);
                if (Mathf.Abs(temp) < minMove)
                {
                    if (temp >= 0)
                        temp = minMove;
                    else
                        temp = -minMove;
                }

                if(movement == MovementType.OnlyLeft)
                {
                    temp = -1.3f * Mathf.Abs(temp);
                } else if(movement == MovementType.OnlyRight)
                {
                    temp = 1.3f * Mathf.Abs(temp);
                } else if(movement == MovementType.Enhanced)
                {
                    temp *= 1.5f;
                }

				pos2 = pos1 + new Vector3 ( temp,0,0);
                float tmpX;
                if (pos2.x > 0)
                    tmpX = pos2.x -0.1f;
                else
                    tmpX = pos2.x + 0.1f; 
                
				isMoving = true;
                isClickable = true;

                MoveTo(pos2, walkDuration);
			}
		}

		void OnMouseDown()
		{
			if (isClickable && onLetterTouched != null) {
                AudioManager.I.PlayLetter(view.Data.Id);
                isClickable = false;
                onLetterTouched (id);
			}
		}

        public void SetMovement(MovementType mov)
        {
            movement = mov;
        }


        #region VARIABLES
        public int id;
		public float idleTime = 0f;
		public float ray = 5f;
		public float minMove = 2.5f;

        private bool isClickable = false;
        
		private bool isMoving = false;
        private bool isArrived = false;
        
		private float startTime;
		private Vector3 pos1;
		private Vector3 pos2;
       
		private Animator anim;

        [HideInInspector]
        public LetterObjectView view;

        public Tweener moveTweener;

        public float walkDuration = 2f;

        private MovementType movement = MovementType.Normal;
        #endregion
    }
}
