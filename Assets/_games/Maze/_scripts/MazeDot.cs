using UnityEngine;
using System.Collections;

namespace EA4S.Maze
{
	public class MazeDot : MonoBehaviour {

		public MazeCharacter character;
		public bool isClicked = false;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnMouseDown()
		{
			isClicked = true;
			MazeGameManager.Instance.timer.StopTimer ();
			MazeGameManager.Instance.currentTutorial.stopCurrentTutorial ();
			character.setClickedDot ();
		}
	}
}
