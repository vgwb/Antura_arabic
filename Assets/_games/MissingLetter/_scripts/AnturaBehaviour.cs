using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.Assertions;


namespace EA4S.MissingLetter
{
    public class AnturaBehaviour : MonoBehaviour {

        [SerializeField]
        private Transform mStart, mEnd;

        private Transform nextPos;
        private AnturaAnimationController mAntura;

        void Start()
        {
            mAntura = GetComponent<AnturaAnimationController>();
            Assert.IsNotNull<AnturaAnimationController>(mAntura, "Add Antura Script to " + name);
            transform.position = mStart.position;
            nextPos = mEnd;
        }

        public void EnterScene(float duration)
        {
            //Old prefab
            //mAntura.BarkWhenRunning = true;
            //mAntura.SetAnimation(AnturaAnim.Run);
            //transform.LookAt(transform.position + Vector3.right * (nextPos.position.x - transform.position.x));
            //transform.DOMove(nextPos.position, duration).OnComplete(delegate {  mAntura.SetAnimation(AnturaAnim.SitBreath); }) ;

            mAntura.State = AnturaAnimationStates.walking;
            mAntura.IsAngry = true;
            mAntura.DoShout();
            transform.LookAt(transform.position + Vector3.left * (nextPos.position.x - transform.position.x));
            transform.DOMove(nextPos.position, duration).OnComplete(delegate { mAntura.State = AnturaAnimationStates.idle; }) ;

            nextPos = nextPos == mStart ? mEnd : mStart;
        }
    }
}
