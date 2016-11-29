using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.SickLetters
{
    public class SickLettersVase : MonoBehaviour
    {

        public BoxCollider vaseCollider;
        public TextMeshPro _counter;
        public int counter
        {
            set
            {
                _counter.text = value.ToString();
                //game.Context.GetOverlayWidget().SetStarsScore(value / (game.targetScale / 3));
            }
            get { return int.Parse(_counter.text);  }
        }

        [HideInInspector]
        public Vector3 vaseStartPose;
        [HideInInspector]
        public Vector3 vaseStartRot;
        [HideInInspector]
        public SickLettersGame game;

        Rigidbody vaseRB;

        // Use this for initialization
        void Start()
        {
            vaseStartPose = transform.position;
            vaseStartRot = transform.eulerAngles;
            vaseRB = GetComponent<Rigidbody>();
        }

        int c = 0;
        SickLettersDraggableDD dd;
        bool cheatingDetected = false;

        void OnTriggerEnter(Collider coll)
        {
            checkEntry(coll);
        }

        void OnTriggerExit(Collider coll)
        {
            if (coll.tag == "Player")
            {
                dd = coll.gameObject.GetComponent<SickLettersDraggableDD>();

                if (!dd || dd.isInVase)
                    return;

                if (dd.isDragging)
                    dd.touchedVase = false;
            }
         }
        /*void OnTriggerStay(Collider coll)
        {
            checkEntry(coll);
        }*/

        private void checkEntry(Collider coll)
        {
            
            if (coll.tag == "Player")
            {
                dd = coll.gameObject.GetComponent<SickLettersDraggableDD>();

                if (dd.isDragging)
                    dd.touchedVase = true;
                if (!dd || dd.isDragging || dd.isInVase || dd.collidedWithVase)
                    return;

                //if (dd.isDragging)
                //  cheatingDetected = true;
                dd.collidedWithVase = true;
                addNewDDToVas(dd);

            }
        }

        public void addNewDDToVas(SickLettersDraggableDD dd)
        {
            

            if (dd.isCorrect)
            {
                game.Poof(dd.transform);
                dd.resetCorrectDD();
                game.onWrongMove();
                StartCoroutine(onDroppingCorrectDD());
                
            }
            else if (!dd.isInVase)
            {
                //dd.deattached = true;

                game.onCorrectMove(dd);

                //game.checkForNextRound();
            }

            
        }

        public IEnumerator onDroppingCorrectDD() {

            if (game.roundsCount == 0)
                yield break;

            StartCoroutine(game.antura.bark());

            yield return new WaitForSeconds(0.5f);

            game.LLPrefab.LLStatus = letterStatus.angry;
            game.LLPrefab.letterView.DoAngry();
            SickLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(game.LLPrefab.letterView.Data, true);

            

            yield return new WaitForSeconds(1.5f);
            game.LLPrefab.LLStatus = letterStatus.idle;

            vaseRB.constraints = RigidbodyConstraints.None;
            vaseRB.isKinematic = false;


            StartCoroutine(dropVase());

            if(game.scale.counter > game.maxWieght)
                game.maxWieght = game.scale.counter;

            game.scale.counter = 0;

            yield return new WaitForSeconds(3);

            game.Poof(transform);
            vaseRB.isKinematic = true;
            transform.position = vaseStartPose + Vector3.up * 20;
            transform.eulerAngles = vaseStartRot;
           

            yield return new WaitForSeconds(1.5f);
            game.antura.sleep();
            
            summonVase();
        }

        public IEnumerator dropVase(float delay = 0, bool moveCam = false)
        {
            //if (game.roundsCount == 0)
              //  yield break;

            yield return new WaitForSeconds(delay);

            vaseRB.constraints = RigidbodyConstraints.None;
            vaseRB.isKinematic = false;

            foreach (SickLettersDraggableDD dd in game.allWrongDDs)
            {
                if (dd && dd.isInVase)
                {
                    dd.boxCollider.isTrigger = false;
                    dd.thisRigidBody.isKinematic = false;
                    dd.thisRigidBody.useGravity = true;
                }
            }

            if (moveCam)
            {
                yield return new WaitForSeconds(0.65f);
                StartCoroutine(game.slCamera.rotatCamera(20f));
            }
        } 

        public void summonVase()
        {
            vaseRB.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            vaseRB.isKinematic = false;
            vaseRB.useGravity = true;
        }

        public void flyVas(float delay = 0)
        {
            StartCoroutine(coFlyVase(delay));
        }

        IEnumerator coFlyVase(float delay)
        {
            yield return new WaitForSeconds(delay);

            foreach (SickLettersDraggableDD dd in game.allWrongDDs)
            {
                if (dd && dd.isInVase)
                {
                    dd.transform.parent = transform;
                    dd.thisRigidBody.isKinematic = true;
                }
            }

            vaseRB.isKinematic = true;

            while (true)
            {
                vaseRB.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 30, 10*Time.deltaTime), transform.position.z);
                game.slCamera.transform.eulerAngles = new Vector3(Mathf.LerpAngle(game.slCamera.transform.eulerAngles.x, -10, 4*Time.deltaTime), game.slCamera.transform.eulerAngles.y, game.slCamera.transform.eulerAngles.z);
                yield return null;
            }
        }
    }
}
