using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class LetterSpawner
    {
        private const float MIN_X = -17f;
        private const float MAX_X = 17f;

        private const float MIN_Y = 0.51f;
        private const float MAX_Y = 0.51f;

        private const float MIN_Z = -8.25f;
        private const float MAX_Z = 13.25f;

        private const float MIN_DISTANCE_SQUARED = 450f;

        public LetterSpawner()
        {

        }

        public Vector3 GetTutorialPosition()
        {
            return new Vector3((MIN_X + MAX_X) / 2 + (MAX_X - MIN_X) * 0.5f, (MIN_Y + MAX_Y) / 2, (MIN_Z + MAX_Z) / 2);
        }

        public Vector3[] GenerateRandomPositions(int numPositions, bool isTutorialLevel)
        {
            Vector3[] randomPositions = new Vector3[numPositions];

            for (int i = 0; i < numPositions; i++)
            {
                Vector3 randomPosition;

                float minDistanceSquared = MIN_DISTANCE_SQUARED;
                int numTries = 0;
                bool isRandomPositionInvalid = true;

                while (isRandomPositionInvalid)
                {
                    if (i == 0 && isTutorialLevel)
                    {
                        randomPosition = GetTutorialPosition();
                    }

                    else
                    {
                        randomPosition = new Vector3(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y), Random.Range(MIN_Z, MAX_Z));
                    }

                    isRandomPositionInvalid = false;

                    for (int j = 0; j < i; j++)
                    {
                        if ((randomPosition - randomPositions[j]).sqrMagnitude <= minDistanceSquared)
                        {
                            isRandomPositionInvalid = true;
                            break;
                        }
                    }

                    if (!isRandomPositionInvalid)
                    {
                        randomPositions[i] = randomPosition;
                    }

                    else
                    {
                        numTries++;
                        if (numTries > 15)
                        {
                            minDistanceSquared *= 0.8f;
                            numTries = 0;
                        }
                    }
                }
            }

            return randomPositions;
        }
    }
}