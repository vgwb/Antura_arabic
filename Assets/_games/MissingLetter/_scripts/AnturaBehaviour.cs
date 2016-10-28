using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.Assertions;


namespace EA4S.MissingLetter
{
    public class AnturaBehaviour : MonoBehaviour {

        private Antura mAntura;

            float xDest = -80;
        // Use this for initialization
        void Start()
        {
            mAntura = GetComponent<Antura>();
            Assert.IsNotNull<Antura>(mAntura, "Add Antura Script to " + name);
        }

        //TODO serialize movement time, switch the antura forward
        public void EnterScene()
        {
            mAntura.BarkWhenRunning = true;
            mAntura.SetAnimation(AnturaAnim.Run);
            transform.LookAt(transform.position + Vector3.right * xDest);
            transform.DOLocalMoveX(xDest, 8).OnComplete(delegate {  mAntura.SetAnimation(AnturaAnim.SitBreath); }) ;
            xDest = -xDest;
        }
    }
}
