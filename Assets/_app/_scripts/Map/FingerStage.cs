using System.Collections;
using Antura.Core;
using UnityEngine;

namespace Antura.Map
{
    public class FingerStage : MonoBehaviour
    {
        public LetterMovement player;
        public bool swipe;
        public GameObject left;
        public GameObject right;
        StageManager stageManager;
        float xDown, xUp, x;
        float yDown, yUp, y;

        void Start()
        {
            stageManager = GetComponent<StageManager>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && (!player.playerOverDotPin)) {
                swipe = true;
                xDown = Input.mousePosition.x;
                yDown = Input.mousePosition.y;
            }
            if (Input.GetMouseButtonUp(0)) {
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


                if ((Mathf.Abs(x) > width * 0.3f) && (Mathf.Abs(y) < height * 0.1f)) {
                    if ((x < 0) && (stageManager.currentStageNumber < AppConstants.MaximumStage)) {
                        stageManager.StageLeft();
                    }
                    if ((x > 0) && (stageManager.currentStageNumber > 1)) {
                        stageManager.StageRight();
                    }
                }
                StartCoroutine("SwipeToFalse");
            }

            // Debug.Log(x);
        }

        IEnumerator SwipeToFalse()
        {
            yield return new WaitForSeconds(0.3f);
            swipe = false;
        }
    }
}