using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine.Assertions;


namespace EA4S.MissingLetter
{
    public class AnturaBehaviour : MonoBehaviour {

        private Antura mAntura;

        //TODO start end position serialize??
        float xDest, xDist;

        // Use this for initialization
        void Start()
        {
            xDest = -50;
            xDist = transform.position.x + 50;
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
            xDest = -xDist;
            xDist *= -1;
        }
    }
}
