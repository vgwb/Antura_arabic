using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class WheelController : MonoBehaviour
    {
        bool isRotating;

        public float initialSpeed;
        float currentSpeed;

        const float brakeForce = 0.98f;
        const float minimalSpeed2Stop = 4f;

        public GameObject TouchParticles;
        Vector3 rotationEuler;

        protected virtual void OnEnable() {
            Lean.LeanTouch.OnFingerTap += OnFingerTap;
        }

        protected virtual void OnDisable() {
            Lean.LeanTouch.OnFingerTap -= OnFingerTap;
        }

        public void OnFingerTap(Lean.LeanFinger finger) {
            // Does the prefab exist?
            if (TouchParticles != null) {
                //Debug.Log("ROTATE");
                isRotating = true;
                currentSpeed = initialSpeed;
                // Clone the prefab, and place it where the finger was tapped
                var position = finger.GetWorldPosition(50.0f);
                var rotation = Quaternion.identity;
                var clone = (GameObject)Instantiate(TouchParticles, position, rotation);

                // Make sure the prefab gets destroyed after some time
                Destroy(clone, 2.0f);
            }
      
        }


        void Update() {
            if (isRotating) {
                rotationEuler += Vector3.forward * currentSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(rotationEuler);

                currentSpeed = currentSpeed * brakeForce;

                if (currentSpeed < minimalSpeed2Stop) {
                    isRotating = false;
                    WheelManager.Instance.OnWheenStopped();
                }

                //To convert Quaternion -> Euler, use eulerAngles
                //print(transform.rotation.eulerAngles);
            }
        }

    }
}