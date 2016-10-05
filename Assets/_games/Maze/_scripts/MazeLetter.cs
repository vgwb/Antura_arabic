using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	public class MazeLetter : MonoBehaviour {


		//the character that should follow the path
		public MazeCharacter character;



		bool isInside;
		bool initialClick;




		// Use this for initialization
		void Start () {


			initialClick = true;
			isInside = false;

		}

		// Update is called once per frame
		void Update () {
			if(isInside) {
				character.calculateMovementAndRotation();
			}
		}

		void OnMouseDown()
		{
			if (character.characterIsMoving || !MazeGameManager.Instance.tutorialForLetterisComplete)
				return;

			//check if input is within range
			if(!character.canMouseBeDown()) return;


			//inform that we are inside the collision
			isInside =  true;

		}



		void OnMouseUp()
		{
			if (!MazeGameManager.Instance.tutorialForLetterisComplete || !isInside)
				return;
			isInside =  false;

			character.initMovement ();
		}






	}

}
