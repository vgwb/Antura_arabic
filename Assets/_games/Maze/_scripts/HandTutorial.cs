using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Maze
{
	
	public class HandTutorial : MonoBehaviour {

		public float handSpeed = 2.0f;

		//support mutliple paths:
		public List<GameObject> pathsToFollow;
		private List<Vector3> wayPoints = new List<Vector3> ();

		private int currentPath = 0;
		private int currentWayPoint = 0;


		private bool isMovingOnPath = false;

		// Use this for initialization
		void Start () {

			//hide the path
			foreach(GameObject pathToFollow in pathsToFollow)
				pathToFollow.SetActive(false);

			gameObject.SetActive (false);
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
					isMovingOnPath = false;

					MazeGameManager.Instance.timer.StartTimer ();

				}
			}


		}

		public void showCurrentTutorial()
		{
			gameObject.SetActive (true);
			isMovingOnPath = true;
			setWayPoints ();
			
		}

		void setWayPoints()
		{
			gameObject.SetActive (true);
			wayPoints = new List<Vector3> ();
			//construct the path waypoints:
			foreach (Transform child in pathsToFollow[currentPath].transform) {
				wayPoints.Add (child.transform.position);
			}
			currentWayPoint = 0;
		}

		public bool isComplete()
		{
			return currentPath == pathsToFollow.Count - 1;
		}

		public void moveToNextPath()
		{
			if (currentPath < pathsToFollow.Count - 1) {
				currentPath++;
				setWayPoints ();
			}
		}

		public bool isCurrentTutorialDone()
		{
			return !isMovingOnPath;
		}


	}

}
