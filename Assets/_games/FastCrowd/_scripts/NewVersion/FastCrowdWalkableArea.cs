using UnityEngine;

public class FastCrowdWalkableArea : MonoBehaviour
{
    BoxCollider[] colliders;

    void Awake()
    {
        colliders = GetComponentsInChildren<BoxCollider>(true);
    }

    public Vector3 GetNearestPoint(Vector2 planePosition)
    {
        for (int i = 0, count = colliders.Length; i < count; ++i)
        {
        }
        return Vector3.zero;
    }
}
