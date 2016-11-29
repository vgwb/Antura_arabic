using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.MixedLetters
{
    public class SeparateLetterController : MonoBehaviour
    {
        // How fast the letter rotates when the rotate button is pressed, in degrees per second:
        private const float ROTATION_SPEED = 720f;

        public Rigidbody rigidBody;
        public BoxCollider boxCollider;

        private float cameraDistance;
        private LL_LetterData letterData;

        [HideInInspector]
        public DropZoneController droppedZone;

        public LetterObjectView letterObjectView;

        private enum State
        {
            NonInteractive, Draggable, Dragging, Rotating
        }

        private State state;

        void Awake()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            boxCollider.enabled = true;
        }

        void Start()
        {
            IInputManager inputManager = MixedLettersConfiguration.Instance.Context.GetInputManager();
            inputManager.onPointerDown += OnPointerDown;
            inputManager.onPointerDrag += OnPointerDrag;
            inputManager.onPointerUp += OnPointerUp;

            cameraDistance = Vector3.Distance(Camera.main.transform.position, transform.position);

            letterObjectView.SetState(LLAnimationStates.LL_still);
            letterObjectView.SetState(LLAnimationStates.LL_limbless);
            
        }

        private void OnPointerDown()
        {
            if (state == State.Draggable)
            {
                Ray ray = Camera.main.ScreenPointToRay(MixedLettersConfiguration.Instance.Context.GetInputManager().LastPointerPosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider == boxCollider)
                {
                    state = State.Dragging;
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

                    MixedLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letterData);
                }
            }
        }

        private void OnPointerDrag()
        {
            if (state == State.Dragging)
            {
                Vector2 lastPointerPosition = MixedLettersConfiguration.Instance.Context.GetInputManager().LastPointerPosition;
                Vector3 pointerPosInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector3(lastPointerPosition.x, lastPointerPosition.y, Mathf.Abs(transform.position.z - Camera.main.transform.position.z)));

                transform.position = pointerPosInWorldUnits;
            }
        }

        private void OnPointerUp()
        {
            if (state == State.Dragging)
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

                state = State.Draggable;
            }
        }

        public bool IsRotating()
        {
            return state == State.Rotating;
        }

        void FixedUpdate()
        {
            rigidBody.AddForce(Constants.GRAVITY, ForceMode.Acceleration);
        }

        public void SetIsKinematic(bool isKinematic)
        {
            rigidBody.isKinematic = isKinematic;
        }

        public void Vanish()
        {
            letterObjectView.Poof();
            Disable();
        }

        public void SetRotation(Vector3 eulerAngles)
        {
            transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        public void SetDraggable()
        {
            state = State.Draggable;
        }

        public void SetNonInteractive()
        {
            state = State.NonInteractive;
        }

        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            rigidBody.AddForce(force, forceMode);
        }

        public void RotateCCW()
        {
            StartCoroutine(RotateCCWCoroutine());
        }

        private IEnumerator RotateCCWCoroutine()
        {
            state = State.Rotating;

            float currentZRotation = transform.localEulerAngles.z;
            float targetZRotation = currentZRotation + 90;

            float increment = Time.fixedDeltaTime * ROTATION_SPEED;

            while (true)
            {
                currentZRotation += increment;

                if (currentZRotation >= targetZRotation)
                {
                    SetRotation(new Vector3(0, 0, targetZRotation % 360));
                    break;
                }

                else
                {
                    SetRotation(new Vector3(0, 0, currentZRotation));
                }

                yield return new WaitForFixedUpdate();
            }

            state = State.Draggable;

            MixedLettersGame.instance.VerifyLetters();
        }

        public void Reset()
        {
            state = State.NonInteractive;
            SetIsKinematic(true);
            SetRotation(new Vector3(0, 180, 0));
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
            letterObjectView.Init(letterData);
        }

        public LL_LetterData GetLetter()
        {
            return letterData;
        }
    }
}