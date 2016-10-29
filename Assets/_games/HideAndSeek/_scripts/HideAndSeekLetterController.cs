using UnityEngine;
using System.Collections;
using DG.Tweening;
using ArabicSupport;
using TMPro;
using EA4S.Db;

namespace EA4S.HideAndSeek
{
public class HideAndSeekLetterController : MonoBehaviour {

		public delegate void TouchAction(int i);
		public static event TouchAction onLetterTouched;
	// Use this for initialization
	void Start () {
			
			//anim = GetComponent<Animator>();
			coll = GetComponent<Collider> ();
            
            
	}
	

		void Update (){

			if (isMoving) 
			{
				if (fase == 0) 
				{
					transform.position = Vector3.Lerp(pos1, pos2, speed * (Time.time - startTime));
					if (transform.position.x == pos2.x && transform.position.y == pos2.y) 
					{
						
						if (idleTime < 1.5f)
						{
							//anim.SetFloat ("speed", 0.8f);
							fase = 2;
							startTime = Time.time;

						}

						else 
						{
							fase++;
							//anim.SetFloat ("speed", 0f);
							timeToWait = Time.time + idleTime;
						}

					}
				}
				else if (fase == 1 && Time.time > timeToWait) 
				{
					
					
					fase++;
					startTime = Time.time;
					//anim.SetFloat("speed",0.8f);

				
				}
				else if (fase == 2) {
					transform.position = Vector3.Lerp (pos2, pos1, (Time.time - startTime));
					if (transform.position.x == pos1.x && transform.position.y == pos1.y) 
					{
						fase = 0;
						//anim.SetFloat("speed",0f);
						isMoving = false;
						coll.enabled = false;


					}
				}
			}
		
		}

        public void SetStartPosition()
        {
            pos1 = transform.position;
        }

		public void Move(){
			if (!isMoving) {
				
                Vector2 temp = Random.insideUnitCircle*ray;
                if (temp.y < 0)
                    temp.y *= -1;
                if (Mathf.Abs(temp.x) < minMove && temp.y < minMove)
                {
                    if (temp.x >= 0)
                        temp.x = minMove;
                    else
                        temp.x = -minMove;
                    temp.y = minMove;
                }


				pos2 = pos1 + new Vector3 ( temp.x,temp.y,0);
				//anim.SetFloat("speed",0.6f);
				startTime = Time.time;
				isMoving = true;
				coll.enabled = true;
			}
		}

		void OnMouseDown()
		{
			if (onLetterTouched != null) {
                AudioManager.I.PlayLetter(view.Model.Data.Key);
                coll.enabled = false;
                onLetterTouched (id);
			}

		}

       

        //var
        public int id;
		public float idleTime = 0f;
		public float ray = 5f;
		public float minMove = 2.5f;


		private int fase = 0;
		private Collider coll;
		private bool isMoving = false;
		private float timeToWait = 0f;
		private float startTime;
		private Vector3 pos1;
		private Vector3 pos2;
		private float speed = 1f;
		private Animator anim;

        public LetterObjectView view;

    }
}
