using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S.Scanner
{
	public class ScannerAntura : MonoBehaviour {


        public static bool  IS_IN_SCENE;

        public static int SCARED_COUNTER;
        public ScannerGame game;
        public Transform stopPose, chargeEndPose;
        public float movingSpeed = 8, chargeSpeed = 11;
        public int timesCanAppear = 1;
        public List<ScannerLivingLetter> fallenLL = new List<ScannerLivingLetter>();

        private AnturaAnimationController antura;
        private Animator anturaAnimator;
        
        int thisRound;
        void Start () {
			antura = GetComponent<AnturaAnimationController>();
            anturaAnimator = GetComponent<Animator>();
            antura.SetWalkingSpeed(1);
            antura.State = AnturaAnimationStates.walking;

            transform.position += Vector3.right * 25;

            StartCoroutine(handleAnturasEvents());

            thisRound = game.roundsManager.numberOfRoundsPlayed;
        }

        
        void Update()
        {
            if(thisRound != game.roundsManager.numberOfRoundsPlayed)
            {
                thisRound = game.roundsManager.numberOfRoundsPlayed;
                fallenLL.Clear();
            }
        }

        IEnumerator handleAnturasEvents()
        {
            yield return new WaitForSeconds(2);

            while (timesCanAppear >0)
            {                
                if (!game.tut.isTutRound)
                {
                    int d = Random.Range(25, 50);
                    print(d);
                    yield return new WaitForSeconds(d);

                    if (game.roundsManager.numberOfRoundsPlayed >= 6)
                        break;

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

            while (transform.position.x < stopPose.position.x + 20)
            {
                transform.position += Vector3.right * chargeSpeed * Time.deltaTime;
                yield return null;
            }

            game.trapDoor.SetBool("TrapUp", false);
            game.trapDoor.SetBool("TrapDown", true);
            
            IS_IN_SCENE = false;
        }

        float sildingDelay;
        IEnumerator throwLL(ScannerLivingLetter ll, float slideTime) {


            Rigidbody rb;
            rb = ll.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward * Random.Range(6, 10) + Vector3.up * Random.Range(12, 15), ForceMode.Impulse);
            ll.letterObjectView.OnJumpStart();
            ll.letterObjectView.OnJumpMaximumHeightReached();

            ll.slidingTime = slideTime;

            yield return new WaitForSeconds(2);
            rb.isKinematic = true;
            rb.useGravity = false;

            StartCoroutine(llReset(ll, slideTime));
                
        }

        void OnTriggerEnter(Collider coll)
        {
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
            float slideTime = 0;
            for (int i = 0; i < game.scannerLL.Count; i++)
            {
                if (game.scannerLL[i].slidingTime > slideTime)
                    slideTime = game.scannerLL[i].slidingTime;
            }

            
            if (game.scannerLL.Count == 3)
                return slideTime + 8;
            else
                return slideTime + 5;
        }

        IEnumerator llReset(ScannerLivingLetter ll, float slideTime)
        {
            //yield return new WaitForSeconds(delay);
            while (Time.time < slideTime)
                yield return null;

            if (fallenLL.IndexOf(ll) >=0  && ll.status == ScannerLivingLetter.LLStatus.None)
            {
                ll.Reset();
                ll.StartSliding();
                fallenLL[fallenLL.IndexOf(ll)] = null;
            }
        }

    }
}