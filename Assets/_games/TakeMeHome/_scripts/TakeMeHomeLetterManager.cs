using UnityEngine;
using System.Collections;
using System;

namespace EA4S.TakeMeHome
{
	
	public class TakeMeHomeLetterManager : MonoBehaviour {
		public GameObject plane;
		public LetterObjectView LLPrefab;
		public event System.Action<ILivingLetterData, bool> onDropped;
		float destroyTimer = 0;

		TakeMeHomeLL dragging;

		TakeMeHomeLL _letter;

		private TakeMeHomeGame game;

		void Start()
		{
			game = GetComponent<TakeMeHomeGame> ();

			game.Context.GetInputManager().onPointerDown += OnPointerDown;
			game.Context.GetInputManager().onPointerUp += OnPointerUp;
			game.Context.GetInputManager().onPointerDrag += OnPointerDrag;
		}

		void OnPointerDown()
		{
			if (_letter != null)
			{
				var pointerPosition = game.Context.GetInputManager().LastPointerPosition;
				var screenRay = Camera.main.ScreenPointToRay(pointerPosition);

				RaycastHit hitInfo;
				if (_letter.GetComponent<Collider>().Raycast(screenRay, out hitInfo,Camera.main.farClipPlane))
				{
					dragging = _letter;

					_letter.OnPointerDown(pointerPosition);
				}
			}
		}

		void OnPointerUp()
		{
			dragging = null;

			if (_letter != null) {

				//check if they match or not
				_letter.OnPointerUp ();

				if (_letter.lastTube != null) {
					

					//for now make a random thing:
					bool win = UnityEngine.Random.Range(0,1.0f) < 0.5f;

					if (win) {
						TakeMeHomeConfiguration.Instance.Context.GetAudioManager ().PlaySound (Sfx.Win);
						game.IncrementScore ();
					}
					else
						TakeMeHomeConfiguration.Instance.Context.GetAudioManager ().PlaySound (Sfx.Lose);


					_letter.lastTube = null;



					game.IncrementRound ();
				}


			}
		}

		void OnPointerDrag()
		{
			if (dragging != null && _letter == dragging)
			{
				var pointerPosition = game.Context.GetInputManager().LastPointerPosition;
				_letter.OnPointerDrag(pointerPosition);
			}
		}

		public void removeLetter()
		{
			if (_letter != null) {
				Destroy (_letter.gameObject);
				_letter = null;
			}
		}

		//uses fast crowd letter management and dragging:
		public void spawnLetter(ILivingLetterData data)
		{
			
			LetterObjectView letterObjectView = Instantiate(LLPrefab);
			letterObjectView.transform.SetParent(transform, true);
			Vector3 newPosition = GetComponent<TakeMeHomeGame> ().spawnTube.transform.position;// = walkableArea.GetFurthestSpawn(letters); // Find isolated spawn point

			letterObjectView.transform.position = newPosition;
			//letterObjectView.transform.rotation = Quaternion.identity
			letterObjectView.Init (data);

			var ll = letterObjectView.gameObject.AddComponent<TakeMeHomeLL>();
			ll.Initialize (plane.transform.position.y,letterObjectView);

			/*/var livingLetter = letterObjectView.gameObject.AddComponent<FastCrowdLivingLetter>();
			//livingLetter.crowd = this;

			letterObjectView.gameObject.AddComponent<FastCrowdDraggableLetter>();*/
			letterObjectView.gameObject.AddComponent<Rigidbody>().isKinematic = true;

			foreach (var collider in letterObjectView.gameObject.GetComponentsInChildren<Collider>())
				collider.isTrigger = true;

			var characterController = letterObjectView.gameObject.AddComponent<CharacterController>();
			characterController.height = 6;
			characterController.center = Vector3.up * 3;
			characterController.radius = 1.5f;



			_letter = (ll);

			/*
			livingLetter.onDropped += (result) =>
			{
				if (result)
				{
					//letters.Remove(livingLetter);
					//toDestroy.Enqueue(livingLetter);
				}

				if (onDropped != null)
					onDropped(letterObjectView.Model.Data, result);
			};*/
		}
	}
}