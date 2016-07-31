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
            gameObject.GetComponent<VignetteAndChromaticAberration>().enabled = status;
            //gameObject.GetComponent<ColorCorrectionCurves>().enabled = status;
        }

        public void SetToPosition(Vector3 newPosition, Quaternion newRotation)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
        }

        public void MoveToPosition(Vector3 newPosition, Quaternion newRotation)
        {
            AudioManager.I.PlaySfx(Sfx.CameraMovement);

            DOTween.Sequence()
                .Append(transform.DOLocalMove(newPosition, 2.0f))
                .Insert(0, transform.DOLocalRotateQuaternion(newRotation, 2.0f))
                .OnComplete(MovementCompleted);
        }

        void MovementCompleted()
        {
            CallbackManager.SendMessage("CameraReady", SendMessageOptions.DontRequireReceiver);
        }

    }
}