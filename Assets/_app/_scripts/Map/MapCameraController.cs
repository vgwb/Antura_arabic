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

        // References
        private StageMapsManager _stageMapsManager;

        // Touch controls
        public bool isSwiping;
        private float xDown, xUp, x;

        private void Start()
        {
            _stageMapsManager = FindObjectOfType<StageMapsManager>();
            isSwiping = false;
        }

        private Transform targetTransform;

        /// <summary>
        /// Follows on the X axis.
        /// Keeps limits based on the stage.
        /// </summary>
        /// <param name="playerPin"></param>
        public void FollowPlayer(PlayerPin playerPin)
        {
            targetTransform = playerPin.transform;
            isFollowing = true;
            // TODO: use a coroutine for this instead
            //CameraGameplayController.I.MoveToPosition(nextCameraPosition, transform.rotation, 0.5f);
            // TODO: stage limits
        }

        public void ManualMovement()
        {
            isFollowing = false;
        }

        private void Update()
        {
            // Player following
            if (isFollowing)
            {
                MoveToLookPosition(targetTransform.position);
                return;
            }

            if (isSwiping) return;

            // Swipe control
            if (Input.GetMouseButtonDown(0))
            {
                xDown = Input.mousePosition.x;
            }

            if (Input.GetMouseButtonUp(0))
            {
                xUp = Input.mousePosition.x;
                x = xDown - xUp;
                xDown = 0;
                xUp = 0;

                float width = Screen.width;

                if (Mathf.Abs(x) > width * 0.3f)
                {
                    if (x < 0) //&& !_stageMapsManager.IsAtFinalStage)
                    {
                        //_stageMapsManager.MoveToNextStageMap();
                        MoveRight();

                        isSwiping = true;
                        StartCoroutine(SwipeCooldownCO());
                    }
                    if (x > 0)//&&  !_stageMapsManager.IsAtFirstStage)
                    {
                        //_stageMapsManager.MoveToPreviousStageMap();
                        MoveLeft();

                        isSwiping = true;
                        StartCoroutine(SwipeCooldownCO());
                    }
                }
            }
        }
        private IEnumerator SwipeCooldownCO()
        {
            yield return new WaitForSeconds(0.3f);
            isSwiping = false;
        }

        public float swipeMovementDistance = 50;

        void MoveLeft()
        {
            MoveToLookPosition(transform.position + Vector3.left * swipeMovementDistance);
        }

        void MoveRight()
        {
            MoveToLookPosition(transform.position + Vector3.right * swipeMovementDistance);
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