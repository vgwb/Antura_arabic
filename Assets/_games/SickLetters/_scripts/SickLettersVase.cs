using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.SickLetters
{
    public class SickLettersVase : MonoBehaviour
    {

        public TextMeshPro _counter;
        public int counter
        {
            set { _counter.text = value.ToString(); }
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
            if (coll.tag == "Player")
            {
                dd = coll.gameObject.GetComponent<SickLettersDraggableDD>();

                if(dd.isDragging)
                    cheatingDetected = true;

                if (dd.isCorrect)
                {
                    game.Poof(dd.transform.position);
                    StartCoroutine(onWrongMove());
                    dd.resetCorrectDD();
                }
                else
                {
                    dd.deattached = true;

                    if (cheatingDetected)
                    {
                        game.Poof(dd.transform.position);
                        Destroy(dd.gameObject);
                        cheatingDetected = false;
                    }
                    else
                    {
                        counter++;
                        dd.isInVase = true;
                    }

                    game.checkForNextRound();
                }
            }
        }


        public IEnumerator onWrongMove() {

            yield return new WaitForSeconds(0.5f);

            game.LLPrefab.LLStatus = letterStatus.angry;
            game.LLPrefab.letterView.DoAngry();
            SickLettersConfiguration.Instance.Context.GetAudioManager().PlayLetterData(game.LLPrefab.letterView.Data, true);

            yield return new WaitForSeconds(1.5f);
            game.LLPrefab.LLStatus = letterStatus.idle;

            vaseRB.constraints = RigidbodyConstraints.None;
            vaseRB.isKinematic = false;


            StartCoroutine(dropVase());

            yield return new WaitForSeconds(3);

            game.Poof(transform.position);
            vaseRB.isKinematic = true;
            transform.position = vaseStartPose + Vector3.up * 20;
            transform.eulerAngles = vaseStartRot;
           

            yield return new WaitForSeconds(1.5f);

            summonVase();
        }

        public IEnumerator dropVase(float delay = 0, bool moveCam = false)
        {
            yield return new WaitForSeconds(delay);

            vaseRB.constraints = RigidbodyConstraints.None;
            vaseRB.isKinematic = false;

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

            while (true)
            {
                vaseRB.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, 30, 10*Time.deltaTime), transform.position.z);
                game.slCamera.transform.eulerAngles = new Vector3(Mathf.LerpAngle(game.slCamera.transform.eulerAngles.x, -10, 4*Time.deltaTime), game.slCamera.transform.eulerAngles.y, game.slCamera.transform.eulerAngles.z);
                yield return null;
            }
        }
    }
}
