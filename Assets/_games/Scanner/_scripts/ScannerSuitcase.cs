using UnityEngine;
using System;
using System.Collections;
using TMPro;


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
		public bool isCorrectAnswer = false;
		public TextMeshPro drawing;
		public GameObject shadow;


		bool overPlayermarker = false;


		public event Action <GameObject> onCorrectDrop;
		public event Action <GameObject> onWrongDrop;

		IEnumerator Coroutine_ScaleOverTime(float time)
		{
			Vector3 originalScale = transform.localScale;
			Vector3 destinationScale = new Vector3(1.0f, 1.0f, 1.0f);

			float currentTime = 0.0f;
			do
			{
				transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
				currentTime += Time.deltaTime;
				yield return null;
			} while (currentTime <= time);
		}

		void Start()
		{

			startX = transform.position.x;
			startY = transform.position.y;
			startZ = transform.position.z;
//			Reset();
		}

		public void Reset()
		{
			transform.position = new Vector3(startX, startY, startZ);
			isDragging = false;
			transform.localScale = new Vector3(0.25f,0.25f,0.25f);
			gameObject.SetActive(true);
			shadow.SetActive(true);
			StartCoroutine(Coroutine_ScaleOverTime(1f));

		}

		void OnMouseDown()
		{
			shadow.SetActive(false);
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

		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player") overPlayermarker = true;
		}

		void OnTriggerStay(Collider other)
		{
			if (other.tag == "Player") overPlayermarker = true;
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player") overPlayermarker = false;
		}

		void OnMouseUp()
		{

			if (overPlayermarker)
			{
				if (isCorrectAnswer)
				{
					onCorrectDrop(gameObject);
				}
				else
				{
					onWrongDrop(gameObject);
				}
			}
			else
			{
				Reset();
			}
			isDragging = false;
			overPlayermarker = false;
		}

	}
}
