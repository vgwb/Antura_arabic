using System;
using System.Collections.Generic;
using UnityEngine;

public class FastCrowdWalkableArea : MonoBehaviour
{
    public GameObject[] spawnPoints;
    Collider[] colliders;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(false);
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


    public Vector3 GetNearestPoint(Vector3 pos)
    {
        Vector3 nearest = pos;
        float nearestDistance = float.PositiveInfinity;

        for (int i = 0, count = colliders.Length; i < count; ++i)
        {
            var nearPos = colliders[i].ClosestPointOnBounds(pos);

            float distance = Vector3.Distance(nearPos, pos);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = nearPos;
            }
        }

        return nearest;
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        foreach (var spawn in spawnPoints)
            Gizmos.DrawSphere(spawn.transform.position, 0.4f);
    }
#endif
}
