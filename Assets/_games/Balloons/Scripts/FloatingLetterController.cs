using UnityEngine;
using System.Collections;

namespace Balloons
{
    public class FloatingLetterController : MonoBehaviour
    {
        [Range(0, 10)]
        public float floatSpeed;
        //e.g. 1f
        [Range(0, 10)]
        public float floatDistance;
        //e.g. 1.5f
        [Range(0, 1)]
        public float floatRandomness;
        // e.g. 0.25f
        [Range(0, 10)]
        public float dragSpeed;
        //e.g. 2f
        [Range(1, 10)]
        public int tapsNeeded;
        // e.g. 3

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
        private Vector3 basePosition;


        void Start()
        {
            basePosition = transform.position;
            RandomizeFloating();
        }

        void Update()
        {
            Float();
        }

        void RandomizeFloating()
        {
            floatSpeed += Random.Range(-floatRandomness * floatSpeed, floatRandomness * floatSpeed);
            floatDistance += Random.Range(-floatRandomness * floatDistance, floatRandomness * floatDistance);
            floatDirection *= (Random.Range(0, 2) > 0 ? -1 : 1);

            Debug.Log(floatSpeed + ", " + floatDistance + ", " + floatDirection);
        }

        void Float()
        {
            transform.position = basePosition + floatDirection * floatDistance * Mathf.Sin(floatSpeed * Time.time) * Vector3.up;
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