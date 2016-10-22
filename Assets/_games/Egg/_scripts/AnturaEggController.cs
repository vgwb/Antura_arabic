using DG.Tweening;
using System;
using UnityEngine;

namespace EA4S.Egg
{
    public class AnturaEggController : MonoBehaviour
    {
        GameObject anturaPrefab;
        Antura antura;

        public Transform enterPosition;
        public Transform exitPosition;

        float barkingTimer = 0.0f;
        bool IsWaken { get { return barkingTimer > 0; } }

        Tween moveTween;

        Action moveEndCallback;

        public void Initialize(GameObject anturaPrefab)
        {
            antura = GameObject.Instantiate(anturaPrefab).GetComponent<Antura>();
            antura.transform.SetParent(transform);
            antura.transform.position = exitPosition.position;
            antura.transform.localEulerAngles = Vector3.zero;
            antura.transform.localScale = Vector3.one;

            antura.ClickToBark = false;
            antura.ClickToChangeDress = false;
        }

        public void Bark()
        {
            //GetComponent<Antura>().IsBarking = true;
            antura.SetAnimation(AnturaAnim.StandExcitedBreath);
            barkingTimer = 3f;
        }
        
        void Update()
        {
            if (IsWaken)
            {
                barkingTimer -= Time.deltaTime;

                if (barkingTimer <= 0)
                {
                    antura.SetAnimation(AnturaAnim.SitBreath);
                }
            }
        }

        public void Enter(Action callback = null)
        {
            Move(enterPosition.position, 1f, callback);
        }

        public void Exit(Action callback = null)
        {
            Move(exitPosition.position, 1f, callback);
        }

        void Move(Vector3 position, float duration, Action callback)
        {
            moveEndCallback = callback;

            moveTween = antura.transform.DOMove(position, duration).OnComplete(delegate () { if (moveEndCallback != null) moveEndCallback(); });
        }
    }
}