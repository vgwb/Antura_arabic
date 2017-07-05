using UnityEngine;

namespace Antura.Animation
{
    public class RotatingJapanSky : MonoBehaviour
    {
        public float speed = 20f;

        Vector3 rotationEuler;

        void Update()
        {
            rotationEuler += Vector3.forward * speed * Time.deltaTime; //increment 30 degrees every second
            transform.rotation = Quaternion.Euler(rotationEuler);

            //To convert Quaternion -> Euler, use eulerAngles
            //print(transform.rotation.eulerAngles);
        }
    }
}