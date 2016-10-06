using UnityEngine;

public class TremblingTube : MonoBehaviour
{
    public bool Trembling;

    float tremblingAmount;
    Vector3 tremblingOffset;
    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Trembling
        Vector3 noise = 0.015f * new Vector3(
            Mathf.Cos(Mathf.Repeat(Time.realtimeSinceStartup * 317, 2 * Mathf.PI)),
            Mathf.Cos(Mathf.Repeat(Time.realtimeSinceStartup * 601, 2 * Mathf.PI)),
            Mathf.Cos(Mathf.Repeat(Time.realtimeSinceStartup * 363, 2 * Mathf.PI)));

        tremblingOffset = Vector3.Lerp(tremblingOffset, noise, 40.0f * Time.deltaTime);

        if (Trembling)
        {
            tremblingAmount = Mathf.Lerp(tremblingAmount, 1.0f, 20.0f * Time.deltaTime);
        }
        else
        {
            tremblingAmount = Mathf.Lerp(tremblingAmount, 0.0f, 5.0f * Time.deltaTime);
        }


        transform.localPosition = Vector3.Lerp(initialPosition, initialPosition + tremblingOffset, tremblingAmount);
    }
}
