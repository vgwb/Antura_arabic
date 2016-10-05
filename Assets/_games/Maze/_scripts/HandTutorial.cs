using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	
	public class HandTutorial : MonoBehaviour {

		public float handSpeed = 2.0f;
		public GameObject pathToFollow;
		private List<Vector3> wayPoints;

		private int currentWayPoint;

		// Use this for initialization
		void Start () {

			wayPoints = new List<Vector3> ();
			//construct the path waypoints:
			foreach (Transform child in pathToFollow.transform) {
				wayPoints.Add (child.transform.position);
			}
			currentWayPoint = 0;

			//hide the path
			pathToFollow.SetActive(false);


		}
		
		// Update is called once per frame
		void Update () {

			//no way points to follow:
			if (wayPoints.Count == 0)
				return;


			//otherwise move to next way point:
			transform.position = Vector3.MoveTowards (transform.position, wayPoints[currentWayPoint], Time.deltaTime*handSpeed);

			if ((transform.position - wayPoints [currentWayPoint]).magnitude == 0.0f) {
				currentWayPoint++;

				//arrived:
				if (currentWayPoint == wayPoints.Count) {
					currentWayPoint = wayPoints.Count - 1;
					gameObject.SetActive (false);

					//set tutorial done:
					MazeGameManager.Instance.tutorialForLetterisComplete = true;

				}
			}


		}


	}

}
