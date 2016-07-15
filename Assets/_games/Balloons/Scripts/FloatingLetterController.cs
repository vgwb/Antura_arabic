using UnityEngine;
using System.Collections;

namespace Balloons
{
    public class FloatingLetterController : MonoBehaviour
    {
        public BalloonVariation[] variations;
        public LetterController letter;

        [Header("Floating Letter Parameters")]
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
        public BalloonVariation ActiveVariation
        {
            get { return _activeVariation; } 
            private set
            {
                _activeVariation = value;
                activeBalloonCount = _activeVariation.balloons.Length;
            }
        }

        private BalloonVariation _activeVariation;

        [HideInInspector]
        public int activeBalloonCount;

        private int floatDirection = 1;
        private float randomOffset = 0f;
        private Vector3 basePosition;


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
            transform.position = basePosition + floatDirection * floatDistance * Mathf.Sin(floatSpeed * Time.time + randomOffset) * Vector3.up;
        }

        public void MoveHorizontally(float x)
        {
            basePosition.x = Mathf.Lerp(basePosition.x, x, dragSpeed * Time.deltaTime);
        }

        public void SetActiveVariation(int index)
        {
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

            if (activeBalloonCount == 0)
            {
                letter.transform.SetParent(null);
                letter.Drop();
                BalloonsGameManager.instance.floatingLetters.Remove(this);
                BalloonsGameManager.instance.OnDropped(letter.isRequired, letter.associatedPromptIndex, letter.LetterModel.Data.Key);
                Destroy(gameObject, 3f);
            }
        }

        public void Disable()
        {
            for (int i = 0; i < ActiveVariation.balloons.Length; i++)
            {
                ActiveVariation.balloons[i].DisableCollider();
            }
            letter.DisableCollider();

        }

    }
}