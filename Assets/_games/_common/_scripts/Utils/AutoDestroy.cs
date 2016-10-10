using UnityEngine;

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
