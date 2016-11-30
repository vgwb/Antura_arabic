using UnityEngine;

namespace EA4S
{
    public class IntroPlantWind : MonoBehaviour
    {
        Quaternion startRotation;
        Vector3 cameraForward;

        float min;
        float max;

        void Start()
        {
            startRotation = transform.rotation;
            cameraForward = Camera.main.transform.forward;

            min = Random.Range(-6f, -2f);
            min = Random.Range(2f, 6f);
        }


        void Update()
        {
            Quaternion newRotation = startRotation;

            float speed = Mathf.Sin(Time.time * 1.2f);

            float angle = Mathf.Lerp(min, max, (speed * speed));

            newRotation = Quaternion.AngleAxis(angle, cameraForward) * newRotation;

            transform.rotation = newRotation;
        }
    }
}