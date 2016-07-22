using UnityEngine;
using System.Collections;

namespace Balloons
{
    public class FloatingLetterController : MonoBehaviour
    {
        public Rigidbody body;
        public BalloonVariation[] variations;

        [Header("Floating Letter Parameters")]
        [Range(-1, 2)] [Tooltip("Index of default variation (-1 for none)")]
        public int defaultActiveVariation;
        [Range(0, 10)] [Tooltip("e.g.: 1")]
        public float floatSpeed;
        [Range(0, 10)] [Tooltip("e.g.: 1.5")]
        public float floatDistance;
        [Range(0, 1)] [Tooltip("e.g.: 0.25")]
        public float floatRandomnessFactor;
        [Range(0, 10)] [Tooltip("e.g.: 3")]
        public float distanceRandomnessMargin;
        [Range(0, 10)] [Tooltip("e.g.: 2")]
        public float dragSpeed;
        [Range(1, 10)] [Tooltip("e.g.: 1")]
        public int tapsNeeded;


        [HideInInspector]
        public LetterController Letter
        {
            get { return ActiveVariation.letter; }
        }

        [HideInInspector]
        public BalloonController[] Balloons
        {
            get { return ActiveVariation.balloons; }
        }

        private BalloonVariation _activeVariation;

        [HideInInspector]
        public BalloonVariation ActiveVariation
        {
            get
            { return _activeVariation; } 
            private set
            {
                _activeVariation = value;
                activeBalloonCount = Balloons.Length;
            }
        }

        private Vector3 _mouseOffset;

        [HideInInspector]
        public Vector3 MouseOffset
        {
            get { return _mouseOffset; }
            set { _mouseOffset.Set(value.x, value.y, 0f); }
        }

        [HideInInspector]
        public int activeBalloonCount;

        private int floatDirection = 1;
        private float randomOffset = 0f;
        private Vector3 basePosition;
        private Vector3 clampedPosition = new Vector3();


        void Awake()
        {
            SetActiveVariation(defaultActiveVariation);
        }

        void Start()
        {
            basePosition = transform.position;
            RandomizePosition();
            RandomizeFloating();
        }

        void Update()
        {
            Float();
        }

        void RandomizePosition()
        {
            basePosition = basePosition + Random.Range(-distanceRandomnessMargin, distanceRandomnessMargin) * Vector3.up;
        }

        void RandomizeFloating()
        {
            randomOffset = Random.Range(0, 2 * Mathf.PI);
            floatSpeed += Random.Range(-floatRandomnessFactor * floatSpeed, floatRandomnessFactor * floatSpeed);
            floatDistance += Random.Range(-floatRandomnessFactor * floatDistance, floatRandomnessFactor * floatDistance);
            floatDirection *= (Random.Range(0, 2) > 0 ? -1 : 1);
        }

        void Float()
        {
            // Float using Rigidbody velocity
            body.velocity = floatDirection * floatDistance * Mathf.Sin(floatSpeed * Time.time + randomOffset) * Vector3.up;

            // Float using Transform position
            //transform.position = basePosition + floatDirection * floatDistance * Mathf.Sin(floatSpeed * Time.time + randomOffset) * Vector3.up;
        }

        public void Drag(Vector3 dragPosition)
        {
            dragPosition.z = transform.position.z;

            // Drag using Transform Position
            transform.position = ClampPositionToStage(dragPosition + MouseOffset);

            // Drag using Rigidbody Position
            //body.MovePosition(ClampPositionToStage(dragPosition + MouseOffset));
        }

        public Vector3 ClampPositionToStage(Vector3 unclampedPosition)
        {
            clampedPosition = unclampedPosition;

            var minX = BalloonsGameManager.instance.minX;
            var maxX = BalloonsGameManager.instance.maxX;
            var minY = BalloonsGameManager.instance.minY;
            var maxY = BalloonsGameManager.instance.maxY;

            clampedPosition.x = clampedPosition.x < minX ? minX : clampedPosition.x;
            clampedPosition.x = clampedPosition.x > maxX ? maxX : clampedPosition.x;
            clampedPosition.y = clampedPosition.y < minY ? minY : clampedPosition.y;
            clampedPosition.y = clampedPosition.y > maxY ? maxY : clampedPosition.y;

            return clampedPosition;
        }

        public void SetActiveVariation(int index)
        {
            if (index < 0)
            {
                return;
            }

            for (int i = 0; i < variations.Length; i++)
            {
                variations[i].gameObject.SetActive(false);
            }
            ActiveVariation = variations[index];
            ActiveVariation.gameObject.SetActive(true);
        }

        public void Pop()
        {
            activeBalloonCount--;

            if (Balloons.Length == 3)
            {
                if (Balloons[0] != null && Balloons[0].balloonCollider.enabled == true)
                {
                    Balloons[0].AdjustMiddleBalloon();
                }
            }

            if (activeBalloonCount <= 0)
            {
                Debug.Log("Pop 2");
                Letter.transform.SetParent(null);
                Letter.Drop();
                BalloonsGameManager.instance.floatingLetters.Remove(this);
                BalloonsGameManager.instance.OnDropped(Letter);
                Destroy(gameObject, 3f);
            }
        }

        public void Explosion()
        {
            Debug.Log("BOOM!");

            float radius = 10f;
            float power = 20f;
            float upwardsModifier = 3f;

            Vector3 explosionPosition = transform.position + 2f * Vector3.up;
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject.GetComponent<FloatingLetterController>() != null)
                {
                    Debug.Log("Hit: " + hit.gameObject.name);
                    Rigidbody hitBody = hit.GetComponent<Rigidbody>();

                    if (hitBody != null)
                    {
                        Debug.Log("Thrusting " + hit.gameObject.name + "!");
                        hitBody.AddExplosionForce(power, explosionPosition, radius, upwardsModifier);
                    }
                }

            }
        }

        public void Disable()
        {
            for (int i = 0; i < Balloons.Length; i++)
            {
                Balloons[i].DisableCollider();
            }
            Letter.DisableCollider();

        }

    }
}