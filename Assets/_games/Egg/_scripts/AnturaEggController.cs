using DG.Tweening;
using System;
using UnityEngine;

namespace EA4S.Egg
{
    public class AnturaEggController : MonoBehaviour
    {
        GameObject anturaPrefab;
        AnturaAnimationController anturaAnimation;

        public Transform enterPosition;
        public Transform exitPosition;

        Tween moveTween;

        Action moveEndCallback;

        public void Initialize(GameObject anturaPrefab)
        {
            anturaAnimation = GameObject.Instantiate(anturaPrefab).GetComponent<AnturaAnimationController>();
            anturaAnimation.transform.SetParent(transform);
            anturaAnimation.transform.position = exitPosition.position;
            anturaAnimation.transform.localEulerAngles = new Vector3(0f, 180f);
            anturaAnimation.transform.localScale = Vector3.one;

            ChengeGameObjectLayer(anturaAnimation.gameObject);
        }

        public void Enter(Action callback = null)
        {
            anturaAnimation.State = AnturaAnimationStates.sucking;
            Move(enterPosition.position, 1f, callback);
        }

        public void Exit(Action callback = null)
        {
            Move(exitPosition.position, 1f, callback);
        }

        void Move(Vector3 position, float duration, Action callback)
        {
            if (moveTween != null)
            {
                moveTween.Kill();
            }

            moveEndCallback = callback;

            moveTween = anturaAnimation.transform.DOMove(position, duration).OnComplete(delegate () { if (moveEndCallback != null) moveEndCallback(); });
        }

        void ChengeGameObjectLayer(GameObject go)
        {
            go.layer = LayerMask.NameToLayer("Ball");

            int childCount = go.transform.childCount;

            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    ChengeGameObjectLayer(go.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}