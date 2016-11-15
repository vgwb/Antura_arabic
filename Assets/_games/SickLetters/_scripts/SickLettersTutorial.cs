using UnityEngine;
using System.Collections;
namespace EA4S.SickLetters
{
    public class SickLettersTutorial : MonoBehaviour {

        public Vector3[] path;
        public bool draw = false;
        public int repeatMax = 3, repeatConter = 0;
        float repeatDely = 3;
        // Use this for initialization
        SickLettersGame game;

        void Start() {
            game = GetComponent<SickLettersGame>();
            StartCoroutine(coDoTutorial());
        }

        // Update is called once per frame
        void Update() {
            /*if (draw)
            {
                doTutorial();
                draw = false;
            }*/
        }

        public void doTutorial(Transform start = null)
        {
            draw = true;
            if (game.roundsCount > 0)
                return;

            if (start)
                path[0] = start.position;

            else
            {
                //TutorialUI.Clear(true);
                foreach (SickLettersDraggableDD dd in game.LLPrefab.thisLLWrongDDs)
                    if (!dd.deattached && dd.transform.parent)
                    {
                        path[0] = dd.transform.position;
                        break;
                    }
            }

            repeatConter = 0;
            //StopCoroutine(coDoTutorial());
            

        }

        IEnumerator coDoTutorial(Transform start = null)
        {
            while (true)
            {
                if (game.roundsCount > 0)
                {
                    TutorialUI.Clear(true);
                    break;
                }
                if (start = null)
                    continue;

                yield return new WaitForSeconds(repeatDely);
                //TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.FingerAndArrow).OnComplete(() => { repeatConter++; StartCoroutine(coDoTutorial());});
                repeatConter++;
                TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.FingerAndArrow);
                
            }

            
            /*else
            {
                //TutorialUI.Clear(true);
                yield break;
            }*/
        }

    }
}