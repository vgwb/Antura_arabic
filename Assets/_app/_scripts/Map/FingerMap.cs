using System.Collections;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// Handles touch to scroll the map right and left
    /// TODO: make it work with the camera
    /// </summary>
    public class FingerMap : MonoBehaviour
    {
        public bool isSwiping;
        private StageMapsManager _stageMapsManager;
        private float xDown, xUp, x;

        private void Start()
        {
            _stageMapsManager = GetComponent<StageMapsManager>();
            isSwiping = false;
        }

        private void Update()
        {
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

                    if (x < 0 && !_stageMapsManager.IsAtFinalStage)
                    {
                        _stageMapsManager.MoveToNextStageMap();

                        isSwiping = true;
                        StartCoroutine(SwipeCooldownCO());
                    }
                    if (x > 0 && !_stageMapsManager.IsAtFirstStage)
                    {
                        _stageMapsManager.MoveToPreviousStageMap();

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
    }
}