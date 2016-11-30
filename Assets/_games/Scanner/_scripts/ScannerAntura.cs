using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Scanner
{
	public class ScannerAntura : MonoBehaviour {


        public static bool PAUSE_NEW_LL_SLIDES, IS_IN_SCENE;
        public static float LAST_NORMAL_SLIDE;
        public static int SCARED_COUNTER;
        public ScannerGame game;
        public Transform stopPose, chargeEndPose;
        public float movingSpeed = 8, chargeSpeed = 11;
        public int timesCanAppear = 1;
        //public bool isScared;


        private AnturaAnimationController antura;
        private Animator anturaAnimator;
        public List<ScannerLivingLetter> fallenLL = new List<ScannerLivingLetter>();
		// Use this for initialization


		void Start () {
			antura = GetComponent<AnturaAnimationController>();
            anturaAnimator = GetComponent<Animator>();
            antura.SetWalkingSpeed(1);
            antura.State = AnturaAnimationStates.walking; //AnturaAnimationStates.sucking;

            transform.position += Vector3.right * 39;

            StartCoroutine(handleAnturasEvents());
        }


        IEnumerator handleAnturasEvents()
        {
            yield return new WaitForSeconds(2);

            while (timesCanAppear >0)
            {                
                if (!game.tut.isTutRound && game.roundsManager.numberOfRoundsPlayed <6)
                {
                    yield return new WaitForSeconds(Random.Range(25, 50));
                    
                    StartCoroutine(enterTheScene());

                    timesCanAppear--;
                }
                yield return null;
            }
        }

        public IEnumerator enterTheScene()
        {
            SCARED_COUNTER = 0;
            IS_IN_SCENE = true;

            game.trapDoor.SetBool("TrapDown", false);
            game.trapDoor.SetBool("TrapUp", true);
 
            antura.transform.eulerAngles = Vector3.up * 90;
            yield return new WaitForSeconds(0);

            antura.State = AnturaAnimationStates.walking;
            antura.SetWalkingSpeed(1);

            while (transform.position.x > stopPose.position.x + 0.01f)
            {
                transform.position -= Vector3.right * movingSpeed * Time.deltaTime;
                yield return null;
            }

            antura.SetWalkingSpeed(0);
            antura.State = AnturaAnimationStates.idle;
            
            StartCoroutine(bark());

            yield return null;
        }

        public IEnumerator bark(float delay = 0)
        {

            antura.IsExcited = true;
            yield return new WaitForSeconds(0.75f);
            antura.IsExcited = false;
            anturaAnimator.SetTrigger("doShout");
            AudioManager.I.PlaySfx(Sfx.DogBarking);
            yield return new WaitForSeconds(2f);
            antura.OnJumpStart();
            yield return new WaitForSeconds(0.5f);
            antura.OnJumpEnded();
            yield return new WaitForSeconds(1.5f);

            if (SCARED_COUNTER > 2)
            {
                SCARED_COUNTER = 0;
                StartCoroutine(leaveScene());
            }

            else
            {
                yield return new WaitForSeconds(1.5f);
                anturaAnimator.SetTrigger("doShout");
                AudioManager.I.PlaySfx(Sfx.DogBarking);
                yield return new WaitForSeconds(2f);
                antura.DoBurp();
                AudioManager.I.PlaySfx(Sfx.DogBarking);
                yield return new WaitForSeconds(0.5f);
                antura.IsAngry = true;

                yield return new WaitForSeconds(2f);
                if (SCARED_COUNTER > 2)
                {
                    SCARED_COUNTER = 0;
                    StartCoroutine(leaveScene(true));
                }
                else
                    charge();
            }
            yield return null;
        }


        void charge()
        {
            PAUSE_NEW_LL_SLIDES = true;
            antura.IsAngry = false;
            antura.IsExcited = false;
            antura.SetWalkingSpeed(1);
            antura.DoCharge(() => { StartCoroutine(chargeMove()); });
        }

        IEnumerator chargeMove()
        {
            AudioManager.I.PlaySfx(Sfx.DogBarking);

            while (transform.position.x > chargeEndPose.position.x + 0.01f)
            {
                transform.position -= Vector3.right * chargeSpeed * Time.deltaTime;
                yield return null;
            }

            
            StartCoroutine(leaveScene());
        }

        IEnumerator leaveScene(bool wasScared = false)
        {
            
            antura.transform.eulerAngles = Vector3.up * 270;
            antura.SetWalkingSpeed(1);
            antura.State = AnturaAnimationStates.walking;

            while (transform.position.x < stopPose.position.x + 30)
            {
                transform.position += Vector3.right * chargeSpeed * Time.deltaTime;
                yield return null;
            }

            /*if (ScannerConfiguration.Instance.Variation != ScannerVariation.OneWord)
                StartCoroutine(resetLetters());

            else if (ScannerConfiguration.Instance.Variation == ScannerVariation.OneWord)
            {
                if (game.scannerLL[0].status == ScannerLivingLetter.LLStatus.None)
                {
                    game.scannerLL[0].StartSliding();//.status = ScannerLivingLetter.LLStatus.Sliding;
                    Debug.LogWarning("VVVV");
                }
            }
            */
            IS_IN_SCENE = false;
        }

        IEnumerator throwLL(ScannerLivingLetter ll, float resetDealy) {


            Rigidbody rb;
            rb = ll.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward * Random.Range(6, 10) + Vector3.up * Random.Range(12, 15), ForceMode.Impulse);
            ll.letterObjectView.OnJumpStart();
            ll.letterObjectView.OnJumpMaximumHeightReached();

            

            yield return new WaitForSeconds(2);
            rb.isKinematic = true;
            rb.useGravity = false;
            ll.status = ScannerLivingLetter.LLStatus.None;

            StartCoroutine(llReset(ll, resetDealy));

            //yield return new WaitForSeconds(2);
            

           /* if (ScannerConfiguration.Instance.Variation == ScannerVariation.OneWord)
            {
                yield return new WaitForSeconds(2);
                StartCoroutine(resetLetters());
            }*/
                
        }

        IEnumerator resetLetters()
        {
            yield break;

            int LLCount = fallenLL.Count;
            for (int i = 0; i < LLCount; i++)
            {
                if (fallenLL[i])
                {
                    //Debug.LogWarning("YYYY");
                    fallenLL[i].Reset(false);
                }
            }
            for (int i = 0; i < LLCount; i++)
            {
                while (Time.time - LAST_NORMAL_SLIDE < calculateDelay()/*2.5f*/)
                    yield return null;

                if (fallenLL[i])
                {
                    //Debug.LogWarning("ZZZZ");
                    fallenLL[i].StartSliding();
                    fallenLL[i] = null;
                    if (game.scannerLL.Count == 3)
                        yield return new WaitForSeconds(8f);
                    else
                        yield return new WaitForSeconds(5f);
                }
            }

            //fallenLL.Clear();
            PAUSE_NEW_LL_SLIDES = false;
        }

        void OnTriggerEnter(Collider coll)
        {
            //print(1);
            ScannerLivingLetter ll = coll.transform.root.GetComponent<ScannerLivingLetter>();
            
            if (ll && !ll.gotSuitcase && ll.status == ScannerLivingLetter.LLStatus.StandingOnBelt)
            {
                if (!fallenLL.Contains(ll))
                {
                    ll.status = ScannerLivingLetter.LLStatus.None;
                    fallenLL.Add(ll);
                    AudioManager.I.PlaySfx(Sfx.LetterSad);
                    StartCoroutine(throwLL(ll, calculateDelay()));
                }
                
            }
        }

        float calculateDelay()
        {

            int llToCome = 0;

            foreach (ScannerLivingLetter ll in game.scannerLL)
            {
                if (ll.status == ScannerLivingLetter.LLStatus.None /*|| ll.status == ScannerLivingLetter.LLStatus.Sliding*/)
                    llToCome++;
            }
            if (game.scannerLL.Count == 3)
            {
                return 8 * (llToCome-1);
                    
            }
            else
            {
                return 5 * (llToCome-1);
            }
        }

        IEnumerator llReset(ScannerLivingLetter ll, float delay)
        {
            yield return new WaitForSeconds(delay);
            ll.Reset();
            ll.StartSliding();
            fallenLL[fallenLL.IndexOf(ll)] = null;
        }
    }
}