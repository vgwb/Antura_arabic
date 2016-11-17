using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.DancingDots
{
    public class DancingDotsTutorial : MonoBehaviour
    {

        public TextMeshPro hitDot;
        public DancingDotsDiacriticPosition[] targetDDs;
        //public Vector3[] path;
        public float repeatDely = 3;

        private int repeatConter = 0;
        private bool doTutOnDots;

        DancingDotsGameManager gameManager;
        Transform source, target;
        Vector3 targetPosition;
        DancingDotsDraggableDot currentDD;

        void Awake()
        {
            gameManager = GetComponent<DancingDotsGameManager>();
        }
        void Start()
        {
            
            StartCoroutine(coDoTutorial());
            
            //warm up
            TutorialUI.DrawLine(-100 * Vector3.up, -100 * Vector3.up, TutorialUI.DrawLineMode.Arrow);
        }


        public void doTutorial()
        {

            if (!gameManager.isTutRound)
                return;

            Debug.Log("Tutorial started");


            doTutOnDots = false;

            foreach (DancingDotsDraggableDot dd in gameManager.dragableDots)
                if (dd.isNeeded && dd.gameObject.activeSelf)
                {
                    currentDD = dd;
                    doTutOnDots = true;
                    source = currentDD.transform;
                    break;
                }


            if (!doTutOnDots)
            {

                foreach (DancingDotsDraggableDot dd in gameManager.dragableDiacritics)
                    if (dd.isNeeded)
                    {
                        currentDD = dd;
                        source = currentDD.transform;
                        break;
                    }

                foreach (DancingDotsDiacriticPosition dd in targetDDs)
                    if (dd.gameObject.activeInHierarchy)
                    {
                        target = dd.transform;
                        break;
                    }
            }

        }

        IEnumerator coDoTutorial()
        {
            while (true)
            {
                if (gameManager.isTutRound && currentDD)
                {

                    yield return new WaitForSeconds(repeatDely);
                    if (currentDD.isDragging || !gameManager.isTutRound)
                        continue;

                    if (doTutOnDots)
                        targetPosition = hitDot.transform.TransformPoint(Vector3.Lerp(hitDot.mesh.vertices[0], hitDot.mesh.vertices[2], 0.5f));
                    else
                        targetPosition = target.position;

                    TutorialUI.DrawLine(source.position - Vector3.forward*2, targetPosition - Vector3.forward*2, TutorialUI.DrawLineMode.FingerAndArrow);

                }

                yield return null;

            }

        }
    }
}