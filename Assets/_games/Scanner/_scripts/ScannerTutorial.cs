using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.Scanner
{
    public class ScannerTutorial : MonoBehaviour
    {
        
        public Transform scannerDevice;
        public ScannerScrollBelt UpperBelt, lowerBelt;
        [HideInInspector]
        public float originalBeltSpeed, originalLLOnBeltSpeed;
        public float startDelay = 8, repeatDelay = 3;

        private bool doTutOnDots;
        private int step = 1;
        ScannerGame gameManager;
        Transform source, target;
        Vector3 targetPosition;
        ScannerSuitcase currentSuitcases;
        
        public bool isTutRound
        {
            get
            {
                if (gameManager.roundsManager.numberOfRoundsPlayed == 0)
                    return true;
                else
                    return false;
            }
        }

        void Awake()
        {
            gameManager = GetComponent<ScannerGame>();
        }
        void Start()
        {            
            StartCoroutine(coDoTutorial());
            originalBeltSpeed = UpperBelt.scrollSpeed;
            originalLLOnBeltSpeed = ScannerConfiguration.Instance.beltSpeed;
            ScannerGame.disableInput = true;
            //warm up
            TutorialUI.DrawLine(-100 * Vector3.up, -100 * Vector3.up, TutorialUI.DrawLineMode.Arrow);
        }


        public void setupTutorial(int step = 1)
        {
            if (!isTutRound)
                return;

            //TutorialUI.Clear(false);
            Debug.Log("Tutorial started");

            this.step = step;
            if(step == 1)
            {
                source = scannerDevice;
            }
            else if (step == 2)
            {
                foreach (ScannerSuitcase sc in gameManager.suitcases)
                    if (sc.isCorrectAnswer)
                    {
                        currentSuitcases = sc;
                        source = sc.transform;
                        target = gameManager.scannerLL.transform;
                        break;
                    }
            }

        }

        void onTutorialStart()
        {
            AudioManager.I.PlayDialog(Db.LocalizationDataId.Scanner_Tuto);
            ScannerConfiguration.Instance.beltSpeed = UpperBelt.scrollSpeed = lowerBelt.scrollSpeed = 0;
            ScannerGame.disableInput = false;
            StartCoroutine(sayTut(repeatDelay));
        }

        void onTutorialEnd()
        {
            TutorialUI.Clear(true);
            ScannerConfiguration.Instance.beltSpeed = originalLLOnBeltSpeed;
            UpperBelt.scrollSpeed = lowerBelt.scrollSpeed = originalBeltSpeed;
            gameManager.Context.GetOverlayWidget().Initialize(true, false, false);
            gameManager.Context.GetOverlayWidget().SetStarsThresholds(gameManager.STARS_1_THRESHOLD, gameManager.STARS_2_THRESHOLD, gameManager.STARS_3_THRESHOLD);

        }

        IEnumerator coDoTutorial()
        {
            yield return new WaitForSeconds(startDelay);

            onTutorialStart();

            while (isTutRound)
            {
                //Debug.Log(1);
                if ((currentSuitcases && currentSuitcases.isDragging && step == 2) || !isTutRound)
                {
                    yield return null;
                    continue;
                }

                if (step == 1)
                    TutorialUI.DrawLine(source.position - Vector3.forward * 2, gameManager.scannerLL.transform.position + new Vector3(-2f, scannerDevice.position.y - gameManager.scannerLL.transform.position.y, -2), TutorialUI.DrawLineMode.FingerAndArrow);
                else
                    TutorialUI.DrawLine(source.position - Vector3.forward * 2, gameManager.scannerLL.transform.position + new Vector3(5f, 3, -2), TutorialUI.DrawLineMode.FingerAndArrow);

                yield return new WaitForSeconds(repeatDelay);

            }


            onTutorialEnd();
        }

        IEnumerator sayTut(float delay)
        {
            yield return new WaitForSeconds(delay);
            //AudioManager.I.PlayDialog("DancingDots_Tuto");
            
            Debug.Log("start Tutorial Dialog");
        }
        
    }
}