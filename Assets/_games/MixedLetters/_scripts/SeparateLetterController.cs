using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.MixedLetters
{
    public class SeparateLetterController : MonoBehaviour
    {
        public TMP_Text TMP_text;
        public Rigidbody rigidBody;
        public BoxCollider boxCollider;

        private bool isBeingDragged = false;
        private bool isDraggable = false;
        private float cameraDistance;
        private LL_LetterData letterData;
        public DropZoneController droppedZone;

        void Start()
        {
            IInputManager inputManager = MixedLettersConfiguration.Instance.Context.GetInputManager();
            inputManager.onPointerDown += OnPointerDown;
            inputManager.onPointerDrag += OnPointerDrag;
            inputManager.onPointerUp += OnPointerUp;

            cameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
        }

        private void OnPointerDown()
        {
            if (!isBeingDragged && isDraggable)
            {
                Ray ray = Camera.main.ScreenPointToRay(MixedLettersConfiguration.Instance.Context.GetInputManager().LastPointerPosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider == boxCollider)
                {
                    isBeingDragged = true;
                    SetIsKinematic(true);

                    if (transform.position.z != DropZoneController.DropZoneZ)
                    {
                        Vector3 position = transform.position;
                        position.z = DropZoneController.DropZoneZ;
                        transform.position = position;
                    }

                    if (droppedZone != null)
                    {
                        droppedZone.SetDroppedLetter(null);
                        droppedZone = null;
                    }

                    MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayLetter(letterData);
                }
            }
        }

        private void OnPointerDrag()
        {
            if (isBeingDragged)
            {
                Vector2 lastPointerPosition = MixedLettersConfiguration.Instance.Context.GetInputManager().LastPointerPosition;
                Vector3 pointerPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(lastPointerPosition.x, lastPointerPosition.y, cameraDistance));

                transform.position = pointerPosInWorldUnits;
            }
        }

        private void OnPointerUp()
        {
            if (isBeingDragged)
            {
                if (DropZoneController.chosenDropZone != null)
                {
                    droppedZone = DropZoneController.chosenDropZone;
                    droppedZone.SetDroppedLetter(this);
                    transform.position = droppedZone.transform.position;
                    DropZoneController.chosenDropZone = null;
                    MixedLettersGame.instance.VerifyLetters();
                }

                else
                {
                    SetIsKinematic(false);
                }

                isBeingDragged = false;
            }
        }

        void FixedUpdate()
        {
            rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);
        }

        public void SetIsKinematic(bool isKinematic)
        {
            rigidBody.isKinematic = isKinematic;
        }

        public void SetRotation(Vector3 eulerAngles)
        {
            transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        public void SetDraggable(bool isDraggable)
        {
            this.isDraggable = isDraggable;
        }

        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            rigidBody.AddForce(force, forceMode);
        }

        public void RotateCCW()
        {
            Vector3 rotation = transform.localEulerAngles;
            rotation.z += 90;
            transform.localRotation = Quaternion.Euler(rotation);
        }

        public void Reset()
        {
            isBeingDragged = false;
            isDraggable = false;
            SetIsKinematic(true);
            SetRotation(new Vector3(0, 0, 0));
            droppedZone = null;
        }

        public void SetPosition(Vector3 position, bool offsetOnZ)
        {
            if (offsetOnZ)
            {
                position.z -= 1f;
            }

            transform.position = position;
        }

        public void SetPosition(Vector3 position)
        {

            transform.position = position;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetLetter(LL_LetterData letterData)
        {
            this.letterData = letterData;
            TMP_text.SetText(letterData.TextForLivingLetter);
        }

        public LL_LetterData GetLetter()
        {
            return letterData;
        }
    }
}