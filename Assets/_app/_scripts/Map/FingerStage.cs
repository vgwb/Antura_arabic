using System.Collections;
using UnityEngine;

namespace Antura.Map
{
    /// <summary>
    /// Handles touch swipes to change stage map
    /// </summary>
    public class FingerStage : MonoBehaviour
    {
        public PlayerPin player;
        public bool isSwiping;
        private StageMapsManager _stageMapsManager;
        private float xDown, xUp, x;
        private float yDown, yUp, y;

        private void Start()
        {
            _stageMapsManager = GetComponent<StageMapsManager>();
            isSwiping = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))// && !player.playerOverDotPin)
            {
                xDown = Input.mousePosition.x;
                yDown = Input.mousePosition.y;
            }

            if (Input.GetMouseButtonUp(0))
            {
                xUp = Input.mousePosition.x;
                yUp = Input.mousePosition.y;
                x = xDown - xUp;
                y = yDown - yUp;
                xDown = 0;
                xUp = 0;
                yDown = 0;
                yUp = 0;

                float width = Screen.width;
                float height = Screen.height;

                if (Mathf.Abs(x) > width * 0.3f && Mathf.Abs(y) < height * 0.1f)
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
            // Debug.Log(x);
        }

        private IEnumerator SwipeCooldownCO()
        {
            yield return new WaitForSeconds(0.3f);
            isSwiping = false;
        }
    }
}