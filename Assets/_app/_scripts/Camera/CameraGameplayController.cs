using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

namespace EA4S
{
    public class CameraGameplayController : MonoBehaviour
    {
        public static CameraGameplayController I;
        public GameObject CallbackManager;
        public bool FxEnabled { get; private set; }

        void Awake()
        {
            I = this;
        }

        void Start()
        {
            EnableFX(AppManager.I.GameSettings.HighQualityGfx);
        }

        public void EnableFX(bool status)
        {
            // Debug.Log("CameraGameplayController EnableFX " + status);
            if (gameObject.GetComponent<VignetteAndChromaticAberration>() != null) {
                FxEnabled = status;
                gameObject.GetComponent<VignetteAndChromaticAberration>().enabled = status;
            }
            //gameObject.GetComponent<ColorCorrectionCurves>().enabled = status;
        }

        public void SetToPosition(Vector3 newPosition, Quaternion newRotation)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
        }

        public void MoveToPosition(Vector3 newPosition, Quaternion newRotation, float duration = 1)
        {
            // Debug.Log("MoveToPosition");
            AudioManager.I.PlaySfx(Sfx.CameraMovementShort);

            DOTween.Sequence()
                .Append(transform.DOLocalMove(newPosition, duration))
                .Insert(0, transform.DOLocalRotate(newRotation.eulerAngles, duration))
                .OnComplete(MovementCompleted);
        }

        void MovementCompleted()
        {
            CallbackManager.SendMessage("CameraReady", SendMessageOptions.DontRequireReceiver);
        }

    }
}