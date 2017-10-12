using System.Collections;
using Antura.CameraControl;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// Controls the new Map camera.
    /// Handles touch to scroll the map right and left.
    /// Makes sure to not conflict with CameraGameplayController (which is now deprecated)
    /// TODO: make it work with the camera
    /// </summary>
    public class MapCameraController : MonoBehaviour
    {
        // State
        private bool isFollowing = false;
        private Transform followedTransform;

        // References
        private StageMapsManager _stageMapsManager;

        // Touch controls
        private bool isSwiping;
        private float xDown;
        public float sensibility = 0.1f;
        public float deceleration = 20.0f;
        public float minScreenPercentage = 0.3f;

        private void Start()
        {
            _stageMapsManager = FindObjectOfType<StageMapsManager>();
            isSwiping = false;
        }

        /// <summary>
        /// Follows the target player over this map
        /// </summary>
        public void FollowPlayer(PlayerPin playerPin)
        {
            isFollowing = true;
            followedTransform = playerPin.transform;
        }

        public void ManualMovement()
        {
            isFollowing = false;
            followedTransform = null;
        }

        private System.DateTime timeDown;
        private void Update()
        {
            // Player following
            if (isFollowing)
            {
                MoveToLookPosition(followedTransform.position);
                return;
            }

            if (isSwiping) return;

            // Swipe control
            if (Input.GetMouseButtonDown(0))
            {
                xDown = Input.mousePosition.x;
                timeDown = System.DateTime.Now;
            }

            if (Input.GetMouseButtonUp(0))
            {
                var xUp = Input.mousePosition.x;
                var xDelta = xDown - xUp;
                xDown = 0;

                var timeUp = System.DateTime.Now;
                var timeDelta = timeDown - timeUp;

                float speed = xDelta / (float)timeDelta.TotalSeconds * sensibility;
                Debug.Log("x " + xDelta + " time " + (float)timeDelta.TotalSeconds);

                float width = Screen.width;
                if (Mathf.Abs(xDelta) > width * minScreenPercentage)
                {
                    StartMoving(speed);
                    isSwiping = true;
                    StartCoroutine(SwipeCooldownCO());
                }
            }
        }
        private IEnumerator SwipeCooldownCO()
        {
            yield return new WaitForSeconds(0.3f);
            isSwiping = false;
        }

        public float swipeMovementDistance = 50;

        void StartMoving(float speed)
        {
            StartCoroutine(StartMovingCO(speed));
            //MoveToLookPosition(transform.position + Vector3.right * swipeMovementDistance);
        }

        IEnumerator StartMovingCO(float speed)
        {
            Debug.Log("MOVE WITH SPEED " + speed);
            Vector3 currentTargetPos = transform.position;
            float currentSpeed = speed;
            int startDir = (int)Mathf.Sign(speed);
            while (currentSpeed != 0.0f)
            {
                Debug.Log(currentSpeed);

                if ((int) Mathf.Sign(currentSpeed) != startDir) 
                {
                    break;
                }

                currentTargetPos += Vector3.right * currentSpeed * Time.deltaTime;
                currentSpeed -= Time.deltaTime * deceleration * startDir;

                if (Mathf.Abs(currentSpeed) <= 10.0f)
                {
                    break;
                }

                // Camera position
                var nextCameraPosition = currentTargetPos;

                // Camera limits
                var startX = _stageMapsManager.CurrentShownStageMap.cameraPivotStart.position.x;
                var endX = _stageMapsManager.CurrentShownStageMap.cameraPivotEnd.position.x;
                var minX = startX < endX ? startX : endX;
                var maxX = startX < endX ? endX : startX;
                if (nextCameraPosition.x > maxX || nextCameraPosition.x < minX)
                {
                    currentSpeed = 0.0f;
                }
                nextCameraPosition.x = Mathf.Clamp(nextCameraPosition.x, minX, maxX);

                // Assign the position
                transform.position = nextCameraPosition;

                yield return null;
            }
            Debug.Log("Finished movement.");
        }

        void MoveToLookPosition(Vector3 lookPosition)
        {
            // Use only the X
            var nextCameraPosition = transform.position;
            nextCameraPosition.x = lookPosition.x;    

            // Camera limits
            var startX = _stageMapsManager.CurrentShownStageMap.cameraPivotStart.position.x;
            var endX = _stageMapsManager.CurrentShownStageMap.cameraPivotEnd.position.x;
            var minX = startX < endX ? startX : endX;
            var maxX = startX < endX ? endX : startX;
            nextCameraPosition.x = Mathf.Clamp(nextCameraPosition.x, minX, maxX);

            // Move
            // TODO: use a coroutine for this instead
            CameraGameplayController.I.MoveToPosition(nextCameraPosition, transform.rotation, 0.5f);
        }

    }
}