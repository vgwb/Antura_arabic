using UnityEngine;
using DG.Tweening;
using EA4S.Audio;
using EA4S.Core;
using UnityStandardAssets.ImageEffects;

namespace EA4S.CameraControl
{

    /// <summary>
    /// Controller for the camera used during gameplay and to show the 3D world.
    /// </summary>
    public class CameraGameplayController : MonoBehaviour
    {
        // refactor: remove the static access
        public static CameraGameplayController I;
        public GameObject CallbackManager;
        public bool FxEnabled { get; private set; }

        void Awake()
        {
            I = this;
        }

        void Start()
        {
            EnableFX(AppManager.Instance.AppSettings.HighQualityGfx);
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
            AudioManager.I.PlaySound(Sfx.CameraMovementShort);

            DOTween.Sequence()
                .Append(transform.DOLocalMove(newPosition, duration))
                .Insert(0, transform.DOLocalRotate(newRotation.eulerAngles, duration))
                .OnComplete(MovementCompleted);
        }

        void MovementCompleted()
        {
            // refactor: can be implemented with an observer pattern instead
            CallbackManager.SendMessage("CameraReady", SendMessageOptions.DontRequireReceiver);
        }

    }
}