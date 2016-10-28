using UnityEngine;
using System.Collections;

namespace EA4S.Maze
{
	public class MazeArrow : MonoBehaviour {

		public bool tweenToColor = false;
		public bool pingPong = false;

		Renderer renderer;
		// Use this for initialization
		void Start () {
			renderer = GetComponent<Renderer> ();
		}

		// Update is called once per frame
		void Update () {
			if (!tweenToColor && !pingPong)
				return;

			if(tweenToColor)
				renderer.material.color = Color.Lerp (renderer.material.color, Color.green, Time.deltaTime * 2);
			else if(pingPong)
				renderer.material.color = Color.Lerp (Color.red, Color.green, Mathf.PingPong(Time.time,1));
		}
	}
}

