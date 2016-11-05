using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


namespace EA4S.SickLetters
{

    public enum letterStatus { idle, angry, horry}

    public class SickLettersLLPrefab : MonoBehaviour
    {

        public TextMeshPro dotlessLetter, correctDot;
        public SickLettersDraggableDD correctDotCollider;
        public SickLettersGame game;
        public LetterObjectView letterView;
        public letterStatus LLStatus = letterStatus.idle;
        public Animator letterAnimator;
        public List<SickLettersDraggableDD> thisLetterDD = new List<SickLettersDraggableDD>();

        string newLetterString = "", prevLetter = ".";
        
        Vector3 statPos;

        // Use this for initialization
        void Start()
        {
            letterView = GetComponent<LetterObjectView>();
            letterAnimator = GetComponent<Animator>();
            statPos = transform.position;
            
        }

        // Update is called once per frame
        void Update()
        {
            //if(LLStatus == letterStatus.idle)
              //  letterAnimator.SetFloat("random", -1);
        }

        public void jumpIn()
        {
            StartCoroutine(coJumpIn());
        }


        public void jumpOut(float delay = 0) {
            StartCoroutine(coJumpOut(delay));
        }

        IEnumerator coJumpIn()
        {

            getNewLetterData();
            scatterDDs();

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<CapsuleCollider>().isTrigger = false;
            letterView.Falling = true;
            yield return new WaitForSeconds(0.30f);

            letterView.OnJumpEnded();
            letterView.SetState(LLAnimationStates.LL_idle);
            letterAnimator.SetBool("idle", true);

            yield return new WaitForSeconds(1f);
            SickLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(letterView.Data, true);
            
        }

        IEnumerator coJumpOut(float delay)
        {
            yield return new WaitForSeconds(delay);
            //GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_idle);
            letterAnimator.Play("LL_idle_1", -1);
            letterView.Falling = true;
            GetComponent<CapsuleCollider>().isTrigger = true;

            yield return new WaitForSeconds(1);

            transform.position = new Vector3(statPos.x, 29.04f ,statPos.z);

            StartCoroutine(coJumpIn());
        }

        public void getNewLetterData()
        {

            ILivingLetterData newLetter;
            prevLetter = newLetterString;
            do
            {
                newLetter = AppManager.Instance.Teacher.GimmeARandomLetter();
                newLetterString = newLetter.TextForLivingLetter.ToString();

            }
            while (newLetterString == "" || game.dotlessLetters.Contains(newLetterString) || newLetterString == prevLetter);
            
            game.LLPrefab.GetComponent<LetterObjectView>().Init(newLetter);
            game.LLPrefab.dotlessLetter.text = newLetterString;
            game.LLPrefab.correctDot.text = newLetterString;

            //correctDotPos = LLPrefab.correctDot.transform.TransformPoint(Vector3.Lerp(LLPrefab.correctDot.mesh.vertices[0], game.LLPrefab.correctDot.mesh.vertices[2], 0.5f));

        }

        int i = 0;
        public void scatterDDs()
        {
            i = 0;
            thisLetterDD.Clear();

            foreach (SickLettersDropZone dz in game.DropZones)
            {
                if (dz.letters.Contains(newLetterString))
                {
                    if (i < game.Draggables.Length)
                    {
                        SickLettersDraggableDD newDragable = game.createNewDragable(game.Draggables[i].gameObject);
                        newDragable.transform.parent = dz.transform;
                        newDragable.transform.localPosition = Vector3.zero;
                        newDragable.transform.localEulerAngles = new Vector3(0, -90, 0);
                        newDragable.setInitPos(newDragable.transform.localPosition);
                        //newDragable.isAttached = true;

                        thisLetterDD.Add(newDragable);
                        i++;
                    }
                }
            }
        }

    }
}
