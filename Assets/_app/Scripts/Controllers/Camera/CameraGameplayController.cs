using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace EA4S
{
    public class CameraGameplayController : MonoBehaviour
    {
        public static CameraGameplayController I;
        public GameObject CallbackManager;

        void Awake() {
            I = this;
        }

        void Start() {
	
        }

        void Update() {
	
        }

        public void GoToPosition(Vector3 newPosition, Quaternion newRotation) {

            DOTween.Sequence()
                .Append(transform.DOLocalMove(newPosition, 2.0f))
                .Insert(0, transform.DOLocalRotateQuaternion(newRotation, 2.0f))
                .OnComplete(MovementCompleted);
        }

        void MovementCompleted() {
            CallbackManager.SendMessage("CameraReady");
        }

    }
}