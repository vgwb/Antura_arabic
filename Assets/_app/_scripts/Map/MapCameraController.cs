using System.Collections;
using System.Threading;
using Antura.Audio;
using Antura.CameraControl;
using DG.Tweening;
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
        enum MovementType
        {
            AUTO,
            MANUAL
        }

        private MovementType movementType;

        // State
        private bool isFollowing = false;
        private Transform followedTransform;

        // References
        private StageMapsManager _stageMapsManager;

        // Touch controls
       // private bool swipeOnCooldown;
        private float xDown;
        private System.DateTime timeDown;
        public float sensibility = 0.1f;
        public float deceleration = 20.0f;
        public float minScreenPercentage = 0.3f;

        private void Start()
        {
            _stageMapsManager = FindObjectOfType<StageMapsManager>();
        }

        #region Movements

        /// <summary>
        /// Follows the target transform.
        /// Mantains the X over the current map
        /// </summary>
        public void SetAutoFollowTransformCurrentMap(Transform targetTransform)
        {
            movementType = MovementType.AUTO;
            followedTransform = targetTransform;
        }

        /// <summary>
        /// Move to a new transform.
        /// Does NOT mantain X over the current map.
        /// Restores manual movement at the end.
        /// </summary>
        public void SetAutoMoveToTransformFree(Transform pivotTr, float duration)
        {
            movementType = MovementType.AUTO;
            followedTransform = null;

            AudioManager.I.PlaySound(Sfx.CameraMovementShort);
            TweenToTransform(pivotTr.position, pivotTr.rotation, duration, SetManualMovementCurrentMap);
        }

        /// <summary>
        /// Restore manual movement.
        /// Forces the movement on the current map.
        /// </summary>
        public void SetManualMovementCurrentMap()
        {
            currentSpeed = 0.0f;
            movementType = MovementType.MANUAL;
            followedTransform = null;
        }

        #endregion

        private void Update()
        {
            if (movementType == MovementType.AUTO)
            {
                // Auto-follow
                if (followedTransform != null)
                {
                    // When slowed down enough, we may re-enable manual movement
                    if (Mathf.Abs(currentSpeed) < 10.0f && Input.GetMouseButtonDown(0))
                    {
                        SetManualMovementCurrentMap();
                        currentSpeed = 0.0f;
                    }
                    else
                    {
                        currentSpeed = (followedTransform.position.x - transform.position.x) * 10.0f;
                        CameraMoveUpdate();
                    }
                }
            }

            if (movementType == MovementType.MANUAL)
            {
                // Manual control
                SwipeControls();
                CameraMoveUpdate();
            }
        }

        private void SwipeControls()
        {
            // Swipe start
            if (Input.GetMouseButtonDown(0))
            {
                xDown = Input.mousePosition.x;
                timeDown = System.DateTime.Now;
            }

            // Swipe end
            if (Input.GetMouseButtonUp(0))
            {
                var xUp = Input.mousePosition.x;
                var xDelta = xDown - xUp;
                xDown = 0;

                var timeUp = System.DateTime.Now;
                var timeDelta = timeDown - timeUp;

                float addedSpeed = xDelta / (float) timeDelta.TotalSeconds * sensibility;
                //Debug.Log("x " + xDelta + " time " + (float)timeDelta.TotalSeconds);

                float width = Screen.width;
                if (Mathf.Abs(xDelta) > width * minScreenPercentage)
                {
                    currentSpeed += addedSpeed;
                }
                else
                {
                    // Stop here
                    currentSpeed = 0.0f;
                }
            }
        }

        private float currentSpeed = 0.0f;
        void CameraMoveUpdate()
        {
            if (currentSpeed != 0.0f)
            {
                //Debug.Log("MOVE WITH SPEED " + currentSpeed);

                //Debug.Log(currentSpeed);
                int startDir = (int)Mathf.Sign(currentSpeed);
                currentSpeed -= Time.deltaTime * deceleration * startDir;
                //Debug.Log("DECEL SPEED: " + currentSpeed);

                if ((int)Mathf.Sign(currentSpeed) != startDir)
                {
                    currentSpeed = 0.0f;
                }

                if (Mathf.Abs(currentSpeed) <= 10.0f)
                {
                    currentSpeed = 0.0f;
                    //followedTransform = null; // Stop following
                }

                Vector3 currentTargetPos = transform.position;
                currentTargetPos += Vector3.right * currentSpeed * Time.deltaTime;

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
            }
            //Debug.Log("Finished movement.");
        }

        private void TweenToTransform(Vector3 newPosition, Quaternion newRotation, float duration = 1, TweenCallback callback = null)
        {
            DOTween.Sequence()
                .Append(transform.DOLocalMove(newPosition, duration))
                .Insert(0, transform.DOLocalRotate(newRotation.eulerAngles, duration))
                .OnComplete(callback);
        }
    }
}