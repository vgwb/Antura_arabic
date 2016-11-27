using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{
	public class ScannerDevice : MonoBehaviour {

		public float minX;
		public float maxX;

		bool isDragging = false;
		private Vector3 screenPoint;
		private Vector3 offset;

		public GameObject goLight;

		public Vector3 fingerOffset;

		public float backDepth = -5f;

		public float depthMovementSpeed = 10f;

		public ScannerGame game;

		private float timeDelta;

		private float frontDepth;

		private bool moveBack = false;

		private bool letterEventsNotSet = true;

		void Start()
		{
			goLight.SetActive(false);

			timeDelta = 0;
			frontDepth = transform.position.z;

		}

		void OnLetterFlying()
		{
			moveBack = true;
		}

		void OnLetterReset()
		{
			moveBack = false;
		}

		void Update()
		{
			if (game.scannerLL != null && letterEventsNotSet)
			{
				letterEventsNotSet = false;
				game.scannerLL.onFlying += OnLetterFlying;
				game.scannerLL.onReset += OnLetterReset;
			}

			if (moveBack && transform.position.z < backDepth)
			{
				transform.Translate(Vector3.forward * depthMovementSpeed * Time.deltaTime); 
			}
			else if (!moveBack && transform.position.z > frontDepth)
			{
				transform.Translate(Vector3.back * depthMovementSpeed * Time.deltaTime); 
			}

		}

		public void Reset()
		{

		}

		void OnMouseDown()
		{
			isDragging = true;
			goLight.SetActive(true);
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

			offset = gameObject.transform.position - 
				Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

		}

		void OnMouseDrag()
		{
			if (isDragging)
			{
				Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
				transform.position = new Vector3 (
					Mathf.Clamp(curPosition.x, minX, maxX),
					transform.position.y, 
					transform.position.z);
			}

		}

		void OnMouseUp()
		{
			isDragging = false;
			goLight.SetActive(false);
			Reset();
		}

		string lastTag = "";

		void OnTriggerEnter(Collider other) 
		{
			if ((other.tag == ScannerGame.TAG_SCAN_START || other.tag == ScannerGame.TAG_SCAN_END) && isDragging)
			{
				if (timeDelta == 0 || lastTag == other.tag)
				{
					timeDelta = Time.time;
					lastTag = other.tag;
				}
				else
				{
					timeDelta = Time.time - timeDelta;
					game.PlayWord(timeDelta);
					timeDelta = 0;
				}
			}
//			else if (other.tag == ScannerGame.TAG_SCAN_END)
//			{
//
//			}

		}
	}
}
