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
		public float depth;

		public ScannerGame game;

		private float timeDelta;

		void Start()
		{
			goLight.SetActive(false);
			timeDelta = 0;
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
			goLight.SetActive(false);
			Reset();
		}

		string lastTag = "";

		void OnTriggerEnter(Collider other) 
		{
			Debug.Log("[Scanner] collider enter");
			if (other.tag == ScannerGame.TAG_SCAN_START || other.tag == ScannerGame.TAG_SCAN_END)
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
