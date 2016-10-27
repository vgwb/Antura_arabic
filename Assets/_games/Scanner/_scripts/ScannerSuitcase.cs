using UnityEngine;
using System.Collections;

namespace EA4S.Scanner
{

	public class ScannerSuitcase : MonoBehaviour {

		bool isDragging = false;
		private Vector3 screenPoint;
		private Vector3 offset;
		private float startX;
		private float startY;
		private float startZ;

		public Vector3 fingerOffset;
		public float depth;

		void Start()
		{

			startX = transform.position.x;
			startY = transform.position.y;
			startZ = transform.position.z;
			Reset();
		}

		public void Reset()
		{
			transform.position = new Vector3(startX, startY, startZ);
			isDragging = false;
			transform.localScale = Vector3.one;
			gameObject.SetActive(true);
		}

		void OnMouseDown()
		{
			isDragging = true;
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
				transform.position = new Vector3 (curPosition.x + fingerOffset.x, curPosition.y + fingerOffset.y, depth);
			}

		}

		void OnMouseUp()
		{
			Reset();
			//		if (overDestinationMarker)
			//		{
			//			if (isDot)
			//			{
			//				DancingDotsGameManager.instance.CorrectDot(); 
			//			}
			//			else
			//			{
			//				DancingDotsGameManager.instance.CorrectDiacritic();
			//			}
			//			gameObject.SetActive(false);
			//		}
			//		else
			//		{
			//			if (overPlayermarker && !isNeeded)
			//			{
			//				DancingDotsGameManager.instance.WrongMove(transform.position);
			//				isDragging = false;
			//				gameObject.SetActive(false);
			//			}
			//			else
			//			{
			//				isDragging = false;
			//
			//				//					StartCoroutine(GoToStartPosition3());
			//			}
			//		}
			//
			//		overPlayermarker = false;
			//		overDestinationMarker = false;
		}

	}
}
