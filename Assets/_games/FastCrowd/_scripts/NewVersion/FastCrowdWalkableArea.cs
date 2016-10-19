using System.Collections.Generic;
using UnityEngine;

public class FastCrowdWalkableArea : MonoBehaviour
{
    public GameObject[] spawnPoints;


    void Awake()
    {
        
    }

    public Vector3 GetFurthestSpawn(List<FastCrowdLivingLetter> letters)
    {
        float bestDistance = -1;
        Vector3 bestSpawn = Vector3.zero;

        foreach (var spawn in spawnPoints)
        {
            var spawnPos = spawn.transform.position;

            float minDistance = float.PositiveInfinity;
            foreach (var letter in letters)
            {
                var letterPosition = letter.transform.position;
                minDistance = Mathf.Min(minDistance, Vector2.Distance(new Vector2(spawnPos.x, spawnPos.z), new Vector2(letterPosition.x, letterPosition.z)));
            }

            if (minDistance > bestDistance)
            {
                bestDistance = minDistance;
                bestSpawn = spawn.transform.position;
            }
        }

        return bestSpawn;
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 randomPosition;
        EA4S.GameplayHelper.RandomPointInWalkableArea(transform.position, 20f, out randomPosition);
        return randomPosition;
    }

    /*
    public Vector3 GetNearestPoint(Vector2 planePosition)
    {
        for (int i = 0, count = colliders.Length; i < count; ++i)
        {
        }
        return Vector3.zero;
    }
    */
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        foreach (var spawn in spawnPoints)
            Gizmos.DrawSphere(spawn.transform.position, 0.4f);
    }
#endif
}
