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
            EnableFX(AppManager.Instance.GameSettings.HighQualityGfx);
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

        public void MoveToPosition(Vector3 newPosition, Quaternion newRotation)
        {
            // Debug.Log("MoveToPosition");
            AudioManager.I.PlaySfx(Sfx.CameraMovement);

            DOTween.Sequence()
                .Append(transform.DOLocalMove(newPosition, 2.0f))
                .Insert(0, transform.DOLocalRotate(newRotation.eulerAngles, 2.0f))
                .OnComplete(MovementCompleted);
        }

        void MovementCompleted()
        {
            CallbackManager.SendMessage("CameraReady", SendMessageOptions.DontRequireReceiver);
        }

    }
}