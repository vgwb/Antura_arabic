using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EA4S.Map
{
    public class FingerStage : MonoBehaviour
    {
        public LetterMovement player;
        public bool swipe;
        public GameObject left;
        public GameObject right;
        /*void Update () {
            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Touch touch = Input.GetTouch(0);
                float x = touch.deltaPosition.x - touch.position.x;
                float y = touch.deltaPosition.y - touch.position.y;

                float width = Screen.width;
                float height = Screen.height;


                if((Mathf.Abs(x) > width * 0.5f) && (Mathf.Abs(y) < height*0.1f))
                {
                    if (x < 0) right.SetActive(true);
                    if ((touch.deltaPosition.x - touch.position.x) > 0) right.SetActive(true);
                }
            }
        }*/
        float xDown, xUp, x;
        float yDown, yUp, y;
        void Update()
        {

            if (Input.GetMouseButtonDown(0) && (!player.playerOverDotPin))
            {
                swipe = true;
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


                if ((Mathf.Abs(x) > width * 0.3f) && (Mathf.Abs(y) < height * 0.1f))
                {
                    if (x < 0) GetComponent<StageManager>().StageLeft();
                    if (x > 0) GetComponent<StageManager>().StageRight();
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
