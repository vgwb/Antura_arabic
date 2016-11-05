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
	// Use this for initialization
	void Start () {

            view = GetComponent<LetterObjectView>();
			//coll = GetComponent<Collider> ();
            
            
	}

        public void resultAnimation(bool win)
        {
            //view.SetState(LLAnimationStates.LL_dancing);
            if (moveTweener != null)
            {
                moveTweener.Kill();
            }

            if(win)
            {
                view.DoDancingWin();
            }
            else
            {
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
                        //coll.enabled = false;
                    }

                        
                    //if (endTransformToCallback != null)
                    //    endTransformToCallback();
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

        void Update (){
            if(isArrived && Time.time > startTime + idleTime )
            {
                MoveTo(pos1, walkDuration / 2);
                isArrived = false;
            }
			/*if (isMoving) 
			{
				if (fase == 0) 
				{
					transform.position = Vector3.Lerp(pos1, pos2, speed * (Time.time - startTime));
                    view.SetState(LLAnimationStates.LL_walking);

                    if (transform.position.x >= posNear.x && transform.position.y >= posNear.y) 
					{
						
						if (idleTime < 1.5f)
						{
                            //anim.SetFloat ("speed", 0.8f);
                            view.SetState(LLAnimationStates.LL_walking);
							fase = 2;
							startTime = Time.time;

						}

						else 
						{
                            view.SetState(LLAnimationStates.LL_idle);
                            fase++;
							//anim.SetFloat ("speed", 0f);
							timeToWait = Time.time + idleTime;
						}

					}
				}
				else if (fase == 1 && Time.time > timeToWait) 
				{

                    view.SetState(LLAnimationStates.LL_walking);
                    fase++;
					startTime = Time.time;
					//anim.SetFloat("speed",0.8f);

				
				}
				else if (fase == 2) {
					transform.position = Vector3.Lerp (pos2, pos1, (Time.time - startTime));
					if (transform.position.x == pos1.x && transform.position.y == pos1.y) 
					{
                        view.SetState(LLAnimationStates.LL_idle);
                        fase = 0;
						//anim.SetFloat("speed",0f);
						isMoving = false;
						coll.enabled = false;


					}
				}
			}*/
		
		}

        public void SetStartPosition(Vector3 pos)
        {
            pos1 = pos;
        }

		public void Move(){
			if (!isMoving) {
				
                float temp = Random.Range(-ray, ray);
               // if (temp.y < 0)
                //    temp.y *= -1;
                if (Mathf.Abs(temp) < minMove/* && temp.y < minMove*/)
                {
                    if (temp >= 0)
                        temp = minMove;
                    else
                        temp = -minMove;
                   // temp.y = minMove;
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
                float tmpX;//, tmpY;
                if (pos2.x > 0)
                    tmpX = pos2.x -0.1f;
                else
                    tmpX = pos2.x + 0.1f; 

                //if (pos2.y > 0)
                //    tmpY = pos2.y - 0.1f;
               // else
               //     tmpY = pos2.y + 0.1f;

               // posNear = new Vector2(tmpX, tmpY);

                //anim.SetFloat("speed",0.6f);
                
				isMoving = true;
                isClickable = true;
				//coll.enabled = true;

                MoveTo(pos2, walkDuration);
			}
		}

		void OnMouseDown()
		{
			if (isClickable && onLetterTouched != null) {
                AudioManager.I.PlayLetter(view.Data.Key);
                //coll.enabled = false;
                isClickable = false;
                onLetterTouched (id);
			}

		}

        public void SetMovement(MovementType mov)
        {
            movement = mov;
        }
       

        //var
        public int id;
		public float idleTime = 0f;
		public float ray = 5f;
		public float minMove = 2.5f;

        private bool isClickable = false;

		//private int fase = 0;
		//private Collider coll;
		private bool isMoving = false;
        private bool isArrived = false;
        //private float timeToWait = 0f;
		private float startTime;
		private Vector3 pos1;
		private Vector3 pos2;
       // private Vector2 posNear;

        //private float speed = 1f;
		private Animator anim;

        [HideInInspector]
        public LetterObjectView view;

        public Tweener moveTweener;

        public float walkDuration = 2f;

        private MovementType movement = MovementType.Normal;
        
    }
}
