using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	public class MazeLetter : MonoBehaviour {


		//the character that should follow the path
		public MazeCharacter character;


		bool isInside;

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
				return;

			//should we replay tutorial?
			if (!isInside) {

                if(MazeGameManager.Instance.isShowingAntura == false)
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
					MazeGameManager.Instance.currentTutorial.isCurrentTutorialDone() == true) {

					idleSeconds += Time.deltaTime;

					if (idleSeconds >= 5) {
						idleSeconds = 0;
						MazeGameManager.Instance.currentTutorial.showCurrentTutorial ();
					}
				}
			}

			
			if(isInside) {
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
