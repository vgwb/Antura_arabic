using UnityEngine;
using System.Collections;
namespace EA4S.SickLetters
{
    public class SickLettersTutorial : MonoBehaviour {

        public Vector3[] path;
        public bool draw = false;
        public float tutorialStartDelay;
        public int  repeatMax = 3, repeatConter = 0;
        float repeatDely = 3;
        // Use this for initialization
        SickLettersGame game;
        SickLettersDraggableDD curDD;

        void Start() {
            game = GetComponent<SickLettersGame>();


            if (!game.enableTutorial)
            {
                game.disableInput = false;
                game.roundsCount = 1;
            }
            else
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
            yield return new WaitForSeconds(tutorialStartDelay);
            game.disableInput = false;
            AudioManager.I.PlayDialog(Db.LocalizationDataId.SickLetters_Tuto);

            while (true)
            {
                if (game.roundsCount > 0)
                {
                    TutorialUI.Clear(true);
                    break;
                }
                /*if (start == null)
                {
                    yield return null;
                    continue;
                }*/

                
                //TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.FingerAndArrow).OnComplete(() => { repeatConter++; StartCoroutine(coDoTutorial());});
                repeatConter++;
                TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.FingerAndArrow);
                yield return new WaitForSeconds(repeatDely);

            }

            
            /*else
            {
                //TutorialUI.Clear(true);
                yield break;
            }*/
        }

        IEnumerator tutDialog(float delay)
        {
            yield return new WaitForSeconds(delay);
            AudioManager.I.PlayDialog("SickLetters_Tuto"); 
        }
    }
}