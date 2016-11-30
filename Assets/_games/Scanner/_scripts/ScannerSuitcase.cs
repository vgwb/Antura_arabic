using UnityEngine;
using System;
using System.Collections;
using TMPro;


namespace EA4S.Scanner
{

	public class ScannerSuitcase : MonoBehaviour {

		public bool isDragging = false, isReady;
		private Vector3 screenPoint;
		private Vector3 offset;
		private float startX;
		private float startY;
		private float startZ;
		private Collider player;
		private Transform originalParent;
        private BoxCollider thisCollider;
        private Rigidbody thisRigidbody;
        private Vector3 originalColliderSize, originalColliderCenter;

        public Vector3 fingerOffset;
		public float scale = 0.5f;
		public bool isCorrectAnswer = false;
		public TextMeshPro drawing;
		public GameObject shadow;

		[HideInInspector]
		public string wordId;


		bool overPlayermarker = false;


		public event Action <GameObject, ScannerLivingLetter> onCorrectDrop;
		public event Action <GameObject> onWrongDrop;

		IEnumerator Coroutine_ScaleOverTime(float time)
		{
            Vector3 originalScale = shadow.transform.localScale;//transform.localScale;
            Vector3 destinationScale = new Vector3(7.6f, 14.167f, 14.167f);//new Vector3(1.0f, 1.0f, 1.0f);

			float currentTime = 0.0f;
			do
			{
                shadow.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
				currentTime += Time.deltaTime;
				yield return null;
			} while (currentTime <= time);
		}

        IEnumerator dropSuitcase()
        {
            yield return new WaitForSeconds(UnityEngine.Random.value / 2);
            thisRigidbody.isKinematic = false;
            thisRigidbody.useGravity = true;
            thisRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            StartCoroutine(Coroutine_ScaleOverTime(1f));

            yield return new WaitForSeconds(1.5f);
            isReady = true;
            thisRigidbody.isKinematic = true;
            thisCollider.size = originalColliderSize;
            thisCollider.center = originalColliderCenter;
        }

		void Start()
		{
			originalParent = transform.parent;
			startX = transform.position.x;
			startY = transform.position.y;
			startZ = transform.position.z;

            thisCollider = GetComponentInChildren<BoxCollider>();
            thisRigidbody = GetComponent<Rigidbody>();
            //Reset();

            originalColliderSize = thisCollider.size;
            originalColliderCenter = thisCollider.center;
            shadow.transform.localScale = Vector3.zero;
            transform.localPosition = Vector3.up * 20;
        }

		public void Reset()
		{
            isReady = false;
            transform.parent = originalParent;
            shadow.transform.localScale = Vector3.zero;
            //transform.position = new Vector3(startX, startY, startZ);
            thisRigidbody.isKinematic = true;
            transform.position = new Vector3(startX, 20, startZ);

            thisCollider.size = new Vector3(originalColliderSize.x, 1, originalColliderSize.z);
            thisCollider.center = new Vector3(originalColliderCenter.x, 0, originalColliderCenter.z);
            isDragging = false;
            //transform.localScale = new Vector3(0.25f,0.25f,0.25f);
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
			shadow.SetActive(true);
            
            StartCoroutine(dropSuitcase());

        }

		void OnMouseDown()
		{
            if (ScannerGame.disableInput || !isReady)
                return;

            shadow.SetActive(false);
			isDragging = true;
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

			offset = gameObject.transform.position - 
				Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

			//transform.localScale = new Vector3(scale,scale,scale);


		}

		void OnMouseDrag()
		{
			if (isDragging)
			{
				Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
				transform.position = 
					new Vector3 (curPosition.x + fingerOffset.x, curPosition.y + fingerOffset.y, transform.position.z);
			}

		}
			
		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player") 
			{ 
				overPlayermarker = true;
				player = other;
			}
		}

		void OnTriggerStay(Collider other)
		{
			if (other.tag == "Player") 
			{
				overPlayermarker = true;
				player = other;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player") 
			{
				overPlayermarker = false;
			}
		}

		void OnMouseUp()
		{
            if (ScannerGame.disableInput || !isReady)
                return;

            if (overPlayermarker)
			{
				ScannerLivingLetter LL = player.transform.parent.GetComponent<ScannerLivingLetter>();
				if (isCorrectAnswer && LL.letterObjectView.Data.Id == wordId)
				{
					LL.gotSuitcase = true;
					transform.parent = player.transform;
					transform.localPosition = new Vector3(5.5f, 1,-2);
					onCorrectDrop(gameObject, LL);
                    transform.localScale = new Vector3(scale, scale, scale);
                    TutorialUI.Clear(true);
                    ScannerTutorial.TUT_STEP = 1;
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
