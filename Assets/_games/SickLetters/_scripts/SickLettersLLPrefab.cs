using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


namespace EA4S.SickLetters
{

    public enum letterStatus { idle, angry, horry}

    public class SickLettersLLPrefab : MonoBehaviour
    {
        public Transform shadow;
        public TextMeshPro dotlessLetter, correctDot;
        public SickLettersDraggableDD correctDotCollider;
        public SickLettersGame game;
        public LetterObjectView letterView;
        public letterStatus LLStatus = letterStatus.idle;
        public Animator letterAnimator;
        public List<SickLettersDraggableDD> thisLLWrongDDs = new List<SickLettersDraggableDD>();


        private SkinnedMeshRenderer[] LLMesh;
        Vector3 statPos, shadowStartSize;


        void Start()
        {
            shadowStartSize = shadow.localScale;
            shadow.localScale = Vector3.zero;
            LLMesh = GetComponentsInChildren<SkinnedMeshRenderer>();
            letterView = GetComponent<LetterObjectView>();
            letterAnimator = GetComponent<Animator>();
            statPos = transform.position;
            
        }


        public void jumpIn()
        {
            StartCoroutine(coJumpIn());
        }


        public void jumpOut(float delay = 0, bool endGame = false) {
            StartCoroutine(coJumpOut(delay, endGame));
        }

        IEnumerator coJumpIn()
        {
            showLLMesh(true);
            getNewLetterData();
            scatterDDs();
            StartCoroutine(fadShadow());

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<CapsuleCollider>().isTrigger = false;
            letterView.Falling = true;
            yield return new WaitForSeconds(0.30f);

            letterView.OnJumpEnded();
            letterAnimator.SetBool("dancing", game.LLCanDance);

            yield return new WaitForSeconds(1f);

            if (game.roundsCount == 0)
            {
                AudioManager.I.PlayDialog("SickLetters_Intro");
                game.tut.doTutorial(thisLLWrongDDs[Random.Range(0, thisLLWrongDDs.Count-1)].transform);
            }
            else
                SickLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letterView.Data, true);
            
        }

        IEnumerator coJumpOut(float delay, bool endGame)
        {

            letterAnimator.SetBool("dancing", false);
            yield return new WaitForSeconds(delay );
            letterAnimator.Play("LL_idle_1", -1);
            game.manager.holeON();
            yield return new WaitForSeconds(0.25f);

            letterView.Falling = true;
            GetComponent<CapsuleCollider>().isTrigger = true;

            yield return new WaitForSeconds(.25f);
            game.Poof(transform.position + Vector3.up *8.5f - Vector3.forward);
            showLLMesh(false);
            yield return new WaitForSeconds(.75f);

            if (!endGame)
            {
                transform.position = new Vector3(statPos.x, 29.04f, statPos.z);
                StartCoroutine(coJumpIn());
            }
        }

        public void getNewLetterData()
        {
            ILivingLetterData newLetter = game.questionManager.getNewLetter();

            game.LLPrefab.GetComponent<LetterObjectView>().Init(newLetter);
            game.LLPrefab.dotlessLetter.text = newLetter.TextForLivingLetter;

            if (!game.LettersWithDots.Contains(newLetter.TextForLivingLetter))
            {
                correctDot.text = "";
                correctDotCollider.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                correctDot.text = newLetter.TextForLivingLetter;
                correctDotCollider.GetComponent<BoxCollider>().enabled = true;
            }

        }

        int i = 0;
        public void scatterDDs(bool isSimpleLetter = true)
        {
            i = 0;
            string letter = "x";

            if (isSimpleLetter)
               letter = game.LLPrefab.dotlessLetter.text;

            foreach (SickLettersDropZone dz in game.DropZones)
            {
                if (dz.letters.Contains(letter))
                {
                    if (i < game.Draggables.Length)
                    {
                        if (game.Draggables[i].diacritic != Diacritic.None && i>=game.numerOfWringDDs/*!game.with7arakat*/)
                        {
                            i++;
                            continue;
                        }
                        SickLettersDraggableDD newDragable = game.createNewDragable(game.Draggables[i].gameObject);
                        newDragable.transform.parent = dz.transform;
                        newDragable.transform.localPosition = Vector3.zero;
                        newDragable.transform.localEulerAngles = new Vector3(0, -90, 0);
                        newDragable.setInitPos(newDragable.transform.localPosition);
                        //newDragable.isAttached = true;

                        thisLLWrongDDs.Add(newDragable);
                        game.allWrongDDs.Add(newDragable);

                        i++;
                    }
                }                
            }

            if (i == 0)
                scatterDDs(false);
        }

        void showLLMesh(bool show)
        {
            foreach (SkinnedMeshRenderer sm in LLMesh)
                sm.enabled = show;
            correctDot.gameObject.SetActive(show);
            dotlessLetter.gameObject.SetActive(show);
            if (!show)
                shadow.localScale = Vector3.zero;
        }

        IEnumerator fadShadow()
        {
            while(shadow.localScale.x < shadowStartSize.x - 0.01f)
            {
                shadow.localScale = Vector3.Lerp(shadow.localScale, shadowStartSize, Time.deltaTime*3.5f);
                yield return null;
            }
        }
    }
}
