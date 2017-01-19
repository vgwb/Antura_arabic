using UnityEngine;

/// <summary>
/// Utility script that automatically destroys a game object after a given duration.
/// </summary>
// refactor: add a namespace
public class AutoDestroy : MonoBehaviour
{
    public float duration;

    void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
