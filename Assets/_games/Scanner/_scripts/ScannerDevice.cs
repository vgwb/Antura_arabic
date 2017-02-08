using UnityEngine;
using System.Collections;

namespace EA4S.Minigames.Scanner
{
	public class ScannerDevice : MonoBehaviour {

		public float minX;
		public float maxX;

		bool isDragging = false;
		private Vector3 screenPoint;
		private Vector3 offset;

		public GameObject goLight;

		public Vector3 fingerOffset;

		public float dragDamping = 10, backDepth = -5f;

		public float depthMovementSpeed = 10f;

		public ScannerGame game;

        public float smoothedDraggingSpeed;

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

		void OnLetterFlying(ScannerLivingLetter sender)
		{
			moveBack = true;
			StopAllCoroutines();
			StartCoroutine(co_Reset());
		}
			
        public float speed = 1;
        public ScannerLivingLetter LL;
        MinigamesCommon.IAudioSource wordSound;

        
        void Update()
		{

            calculateSmoothedSpeed();

            if (game.scannerLL.Count != 0 && letterEventsNotSet)
			{
				letterEventsNotSet = false;
				foreach (ScannerLivingLetter LL in game.scannerLL)
				{
					LL.onFlying += OnLetterFlying;
				}
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

        
        float curPos, prevPose;
        float[] values = new float[16];
        int i = 0;
        void calculateSmoothedSpeed()
        {
            prevPose = curPos;
            curPos = Mathf.Abs(transform.position.x);

            values[i] = Mathf.Abs(curPos - prevPose);
            i++;
            if (i == values.Length)
                i = 0;

            for (int x = 0; x < values.Length; x++)
                smoothedDraggingSpeed = smoothedDraggingSpeed + values[x];

            smoothedDraggingSpeed = (smoothedDraggingSpeed / values.Length);

            //draggingSpeed = Mathf.SmoothDamp( draggingSpeed, Mathf.Abs(curPos - prevPose), ref draggingSpeed, Time.deltaTime);// time;
            //draggingSpeed = (draggingSpeed + prevPose) / 2;
            //Debug.LogError(smoothedDraggingSpeed);
        }

        public void Reset()
		{
			moveBack = false;
		}

		IEnumerator co_Reset()
		{
			yield return new WaitForSeconds(3.25f);
			moveBack = false;
		}

		void OnMouseDown()
		{
            if (game.disableInput)
                return;

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
				transform.position = Vector3.Lerp(transform.position, new Vector3 (
					Mathf.Clamp(curPosition.x, minX, maxX),
					transform.position.y, 
					transform.position.z), Time.deltaTime * dragDamping);
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
					ScannerLivingLetter LL = other.transform.parent.GetComponent<ScannerLivingLetter>();
					timeDelta = Time.time - timeDelta;
                    //Debug.LogError(timeDelta);
					game.PlayWord(timeDelta, LL);
					timeDelta = 0;

                    if(game.tut.tutStep == 1)
                        game.tut.setupTutorial(2, LL);
                }
			}
            //			else if (other.tag == ScannerGame.TAG_SCAN_END)
            //			{
            //
            //			}

            if (other.gameObject.name.Equals("Antura") && isDragging)
            {
                game.antura.GetComponent<ScannerAntura>().scaredCounter++;
            }
		}
	}
}
