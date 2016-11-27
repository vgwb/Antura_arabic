using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	public class MazeLetter : MonoBehaviour {


		//the character that should follow the path
		public MazeCharacter character;


		public bool isInside;

		public float idleSeconds = 0;

        public float anturaSeconds;

		// Use this for initialization
		void Start () {
            anturaSeconds = 0;
            isInside = false;
			character.toggleVisibility (false);
			//character.gameObject.SetActive (false);
		}

		// Update is called once per frame
		void Update () {
           
			if (character.characterIsMoving)
            {
                anturaSeconds = 0;
                return;
            }

			//should we replay tutorial?
			if (!isInside) {

                if (!MazeGameManager.Instance.currentCharacter || MazeGameManager.Instance.currentCharacter.isFleeing || MazeGameManager.Instance.currentCharacter.isAppearing)
                    return;

                if(MazeGameManager.Instance.currentTutorial && MazeGameManager.Instance.currentTutorial.isShownOnce && MazeGameManager.Instance.isShowingAntura == false)
                {
                    anturaSeconds += Time.deltaTime;

                    if (anturaSeconds >= 10)
                    {
                        anturaSeconds = 0;
                        MazeGameManager.Instance.onIdleTime();
                    }

                }


                if (MazeGameManager.Instance.currentTutorial != null && 
					MazeGameManager.Instance.currentTutorial.isStopped == false &&
					MazeGameManager.Instance.currentTutorial.isShownOnce == true) {

					idleSeconds += Time.deltaTime;

					if (idleSeconds >= 5) {
						idleSeconds = 0;
						MazeGameManager.Instance.currentTutorial.showCurrentTutorial ();
					}
				}
			}

			
			if(isInside) {
                anturaSeconds = 0;

                character.calculateMovementAndRotation();
			}
		}

		void OnMouseDown()
		{

			//if (!MazeGameManager.Instance.tutorialForLetterisComplete ()) {
				//force:
				
			//}

			if (character.characterIsMoving/* || !MazeGameManager.Instance.tutorialForLetterisComplete()*/)
				return;

			//check if input is within range
			if(!character.canMouseBeDown()) return;

            Debug.Log("started Drawing!");

			idleSeconds = 0;
			MazeGameManager.Instance.currentTutorial.stopCurrentTutorial();
            anturaSeconds = 0;
			//inform that we are inside the collision
			isInside =  true;

		}



		void OnMouseUp()
		{
			if (!MazeGameManager.Instance.tutorialForLetterisComplete() || !isInside)
				return;
			isInside =  false;
			character.toggleVisibility (true);
			//character.gameObject.SetActive (true);
			character.initMovement ();

			MazeGameManager.Instance.timer.StopTimer ();
		}

	}

}
