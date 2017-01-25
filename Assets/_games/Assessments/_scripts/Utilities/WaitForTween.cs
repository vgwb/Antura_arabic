using DG.Tweening;
using Kore.Coroutines;
using System;
using UnityEngine;

namespace EA4S.Assessment
{
    class WaitForTween : IYieldable, ICustomYield
    {
        //bool done = false;
        Tweener tweenToWait = null; 

        public WaitForTween(Tweener tween)
        {
            if (tween == null)
                throw new ArgumentNullException();

            tweenToWait = tween;
        }

        public bool HasDone()
        {
            if (!tweenToWait.IsPlaying())
                Debug.Log("Tween Completed");
            return !tweenToWait.IsPlaying();
        }

        public void OnYield(ICoroutineEngine engine)
        {
            engine.RegisterCustomYield(this);
        }

        public void Update( Method method)
        {

        }
    }
}
