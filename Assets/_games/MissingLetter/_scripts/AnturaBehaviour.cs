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
        private Antura mAntura;

        void Start()
        {
            mAntura = GetComponent<Antura>();
            Assert.IsNotNull<Antura>(mAntura, "Add Antura Script to " + name);
            transform.position = mStart.position;
            nextPos = mEnd;
        }

        public void EnterScene(float duration)
        {
            mAntura.BarkWhenRunning = true;
            mAntura.SetAnimation(AnturaAnim.Run);
            transform.LookAt(transform.position + Vector3.right * (nextPos.position.x - transform.position.x));
            transform.DOMove(nextPos.position, duration).OnComplete(delegate {  mAntura.SetAnimation(AnturaAnim.SitBreath); }) ;
            nextPos = nextPos == mStart ? mEnd : mStart;
        }
    }
}
