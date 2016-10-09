using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;


namespace EA4S.Maze
{
	public class MazeGameManager : MiniGameBase
	{
		
		public static MazeGameManager Instance;
		public MazeGameplayInfo GameplayInfo;

		public MazeCharacter currentCharacter;
		public HandTutorial currentTutorial;

		public List<GameObject> prefabs;



		int currentLetterIndex;
		GameObject currentPrefab;
		public int health = 4;
		public GameObject cracks;
		List<GameObject> _cracks;
		List<GameObject> lines;

		protected override void Awake()
		{
			base.Awake();
			Instance = this;
		}

		protected override void Start()
		{
			base.Start();

			AppManager.Instance.InitDataAI();
			AppManager.Instance.CurrentGameManagerGO = gameObject;
			SceneTransitioner.Close();



			//cracks to display:
			_cracks = new List<GameObject> ();
			cracks.SetActive (true);
			foreach (Transform child in cracks.transform) {
				child.gameObject.SetActive (false);
				_cracks.Add (child.gameObject);
			}
			lines = new List<GameObject>();

			currentLetterIndex = 0;

			//init first letter
			initCurrentLetter();
		}

		protected override void ReadyForGameplay()
		{
			base.ReadyForGameplay();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnMinigameQuit()
		{
			base.OnMinigameQuit();
		}

		public bool tutorialForLetterisComplete()
		{
			return currentTutorial.isCurrentTutorialDone ();
		}

		public bool isCurrentLetterComplete()
		{
			return currentTutorial.isComplete ();
		}

		public void wasHit()
		{
			_cracks [_cracks.Count- health].SetActive (true);
			health--;

			StartCoroutine (shakeCamera (0.5f, 0.5f));

		}

		public void moveToNext()
		{
			//check if current letter is complete:
			if (currentCharacter.isComplete ()) {
				currentLetterIndex++;
				if (currentLetterIndex == prefabs.Count - 1) {
					print ("all done");
					return;
				}else
				restartCurrentLetter ();
			} else {
				currentCharacter.nextPath ();
				currentTutorial.moveToNextPath ();
			}
		}

		public void restartCurrentLetter()
		{
			Destroy (currentPrefab);
			initCurrentLetter ();
		


			hideCracks ();
			removeLines ();
		}

		void removeLines()
		{
			foreach (GameObject line in lines)
				Destroy (line);
			lines = new List<GameObject>();
		}

		void hideCracks()
		{
			health = 4;
			//hide cracks:
			foreach (Transform child in cracks.transform) {
				child.gameObject.SetActive (false);
			}
		}

		void initCurrentLetter()
		{
			currentPrefab = (GameObject)Instantiate(prefabs[currentLetterIndex],Vector3.zero, Quaternion.identity);
			foreach (Transform child in currentPrefab.transform) {
				if (child.name == "Mazecharacter")
					currentCharacter = child.GetComponent<MazeCharacter> ();
				else if(child.name == "HandTutorial")
					currentTutorial = child.GetComponent<HandTutorial> ();
			}

			//
			if(currentTutorial != null)
				currentTutorial.showCurrentTutorial();
		}

		IEnumerator shakeCamera(float duration, float magnitude) {

			float elapsed = 0.0f;

			Vector3 originalCamPos = Camera.main.transform.position;

			while (elapsed < duration) {

				elapsed += Time.deltaTime;          

				float percentComplete = elapsed / duration;         
				float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

				// map value to [-1, 1]
				float x = UnityEngine.Random.value * 2.0f - 1.0f;
				float y = UnityEngine.Random.value * 2.0f - 1.0f;
				x *= magnitude * damper;
				y *= magnitude * damper;

				Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);

				yield return null;
			}

			Camera.main.transform.position = originalCamPos;
		}

		public void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			start.z = end.z = -0.5f;//-0.1f;
			GameObject myLine = new GameObject();
			myLine.transform.position = start;
			myLine.AddComponent<LineRenderer>();
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = new Material(Shader.Find("Unlit/Color"));
			lr.SetColors(color, color);

			lr.SetWidth(0.3f, 0.3f);
			lr.SetPosition(0, start);
			lr.SetPosition(1, end);

			lines.Add(myLine);
		}
	}

	[Serializable]
	public class MazeGameplayInfo : AnturaGameplayInfo
	{
		[Tooltip("Play session duration in seconds.")]
		public float PlayTime = 0f;
	}
}
