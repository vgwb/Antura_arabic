using UnityEngine;
using System.Collections;

namespace Balloons
{
    public class FloatingLetterController : MonoBehaviour
    {
        [Range(0, 10)] //e.g. 1f
        public float floatSpeed;

        [Range(0, 10)] //e.g. 1.5f
        public float floatDistance;

        [Range(0, 1)] // e.g. 0.25f
        public float floatRandomnessFactor;

        [Range(0, 10)] //e.g. 3f
        public float distanceRandomnessMargin;

        [Range(0, 10)] //e.g. 2f
        public float dragSpeed;

        [Range(1, 10)] // e.g. 1
        public int tapsNeeded;


        public BalloonVariation[] variations;

        private BalloonVariation _activeVariation;

        [HideInInspector]
        public BalloonVariation ActiveVariation
        {
            get { return _activeVariation; } 
            private set
            {
                _activeVariation = value;
                activeBalloonCount = _activeVariation.balloonTops.Length;
            }
        }


        [HideInInspector]
        public int activeBalloonCount;

        public LetterController letter;

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
                if (letter.isRequired)
                {
                    BalloonsGameManager.instance.OnPoppedRequired(letter.associatedPromptIndex);
                }
                letter.transform.SetParent(null);
                letter.Drop();
                BalloonsGameManager.instance.balloons.Remove(this);
                BalloonsGameManager.instance.OnPoppedGroup();
                Destroy(gameObject, 3f);
            }
        }

    }
}