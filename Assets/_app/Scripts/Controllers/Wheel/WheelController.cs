using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class WheelController : MonoBehaviour
    {
        [HideInInspector]
        public bool isRotating;

        public float initialSpeed;
        float currentSpeed;

        const float brakeForce = 0.98f;
        const float minimalSpeed2Stop = 4f;

        Vector3 rotationEuler;

        protected virtual void OnEnable() {
            // Hook into the OnSwipe event
            Lean.LeanTouch.OnFingerSwipe += OnFingerSwipe;
            // Lean.LeanTouch.OnFingerTap += OnFingerTap;
        }

        protected virtual void OnDisable() {
            // Unhook into the OnSwipe event
            Lean.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            // Lean.LeanTouch.OnFingerTap -= OnFingerTap;
        }

        public void OnFingerSwipe(Lean.LeanFinger finger) {
            var swipe = finger.SwipeDelta;
            if (swipe.y < -Mathf.Abs(swipe.x)) {
                StartWheel();
            } else {
                Debug.Log("SWIPE UP YOU MUST!");
            }
        }

        public void OnFingerTap(Lean.LeanFinger finger) {
            StartWheel();
        }

        void StartWheel() {
            if (isRotating == false) {
                //Debug.Log("ROTATE");
                isRotating = true;
                currentSpeed = initialSpeed + Random.Range(-200, 200);
                WheelManager.Instance.OnWheelStart();
            }
        }


        void Update() {
            if (isRotating) {
                rotationEuler -= Vector3.forward * currentSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(rotationEuler);

                currentSpeed = currentSpeed * brakeForce;

                if (currentSpeed < minimalSpeed2Stop) {
                    isRotating = false;
                    WheelManager.Instance.OnWheelStopped();
                }
            }
        }

    }
}