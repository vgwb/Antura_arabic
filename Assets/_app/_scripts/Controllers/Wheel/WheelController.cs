using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class WheelController : MonoBehaviour
    {
        [HideInInspector]
        public bool isRotating;
        public bool isQuiteStopped;
        public float initialSpeed;
        [HideInInspector]
        public float currentSpeed;

        const float brakeForce = 0.98f;
        const float brakeForceEnhanced = 0.82f;
        const float minimalSpeed2Stop = 2f;

        /// <summary>
        /// Set true to force brake wheel almost immediately (it depends to brakeForceEnhanced const value).
        /// </summary>
        public bool IsBrakeForceEnhanced = false;

        bool firstRoundDone;

        Vector3 rotationEuler;

        protected virtual void OnEnable()
        {
            firstRoundDone = false;
            // Hook into the OnSwipe event
            Lean.LeanTouch.OnFingerSwipe += OnFingerSwipe;
            // Lean.LeanTouch.OnFingerTap += OnFingerTap;
        }

        protected virtual void OnDisable()
        {
            // Unhook into the OnSwipe event
            Lean.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            // Lean.LeanTouch.OnFingerTap -= OnFingerTap;
        }

        public void OnFingerSwipe(Lean.LeanFinger finger)
        {
            var swipe = finger.SwipeDelta;
            if (swipe.y < -Mathf.Abs(swipe.x)) {
                StartWheel();
            } else {
                Debug.Log("SWIPE UP YOU MUST!");
            }
        }

        //        public void OnFingerTap(Lean.LeanFinger finger) {
        //            StartWheel();
        //        }

        void StartWheel()
        {
            if (!firstRoundDone && !isRotating) {
                //Debug.Log("ROTATE");
                firstRoundDone = true;
                isRotating = true;
                isQuiteStopped = false;
                currentSpeed = initialSpeed + Random.Range(-200, 200);
                WheelManager.Instance.OnWheelStart();
            }
        }


        void FixedUpdate()
        {
            if (isRotating) {
                rotationEuler -= Vector3.forward * currentSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(rotationEuler);

                currentSpeed = currentSpeed * (IsBrakeForceEnhanced ? brakeForceEnhanced : brakeForce);

                if (currentSpeed < 200f && !isQuiteStopped) {
                    AudioManager.I.StopSfx(Sfx.WheelStart);
                    isQuiteStopped = true;
                }

                if (currentSpeed < minimalSpeed2Stop) {
                    isRotating = false;
                    WheelManager.Instance.OnWheelStopped();
                    IsBrakeForceEnhanced = false;
                }
            }
        }
    }
}