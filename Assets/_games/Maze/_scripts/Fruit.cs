using UnityEngine;

namespace EA4S.Minigames.Maze
{
	
	public class Fruit : MonoBehaviour {


		public float rotationSpeed = 2.0f;


		// Update is called once per frame
		void Update () {
			transform.Rotate (0, Time.deltaTime * rotationSpeed, 0);
		}
	}
}