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

		private Transform parentTransform;

		void Start()
		{
			goLight.SetActive(false);
			parentTransform = transform.parent.transform;
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
				parentTransform.position = new Vector3 (
					Mathf.Clamp(curPosition.x, minX, maxX),
					parentTransform.position.y, 
					parentTransform.position.z);
			}

		}

		void OnMouseUp()
		{
			goLight.SetActive(false);

			Reset();
		}

	}
}
