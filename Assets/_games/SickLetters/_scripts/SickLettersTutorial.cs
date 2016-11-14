using UnityEngine;
using System.Collections;
namespace EA4S.SickLetters
{
    public class SickLettersTutorial : MonoBehaviour {

        public Vector3[] path;
        public bool draw = false;
        public int repeatMax = 3, repeatConter = 0;
        float repeatDely = 1;
        // Use this for initialization
        SickLettersGame game;

        void Start() {
            game = GetComponent<SickLettersGame>();
            
        }

        // Update is called once per frame
        void Update() {
            if (draw)
            {
                doTutorial();
                draw = false;
            }
        }

        public void doTutorial(Transform start = null)
        {
            if (game.roundsCount > 0)
                return;

            if (start)
                path[0] = start.position;

            else
            {
                TutorialUI.Clear(true);
                foreach (SickLettersDraggableDD dd in game.LLPrefab.thisLLWrongDDs)
                    if (!dd.deattached && dd.transform.parent)
                    {
                        path[0] = dd.transform.position;
                        break;
                    }
            }

            repeatConter = 0;
            StopCoroutine(coDoTutorial());
            StartCoroutine(coDoTutorial());

        }

        IEnumerator coDoTutorial()
        {
            if (repeatConter <= repeatMax)
            {
                yield return new WaitForSeconds(repeatDely);
                TutorialUI.DrawLine(path, TutorialUI.DrawLineMode.FingerAndArrow).OnComplete(() => { repeatConter++; StartCoroutine(coDoTutorial());});
            }
            else
            {
                //TutorialUI.Clear(true);
                yield break;
            }
        }

    }
}