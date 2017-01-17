using Kore.Coroutines;
using System;
using System.Collections;
using UnityEngine;

namespace EA4S.Assessment
{
    class WaitCoroutine : IYieldable, ICustomYield
    {
        bool done = false;

        public WaitCoroutine( IEnumerator enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException();

            Koroutine.Run( ParallelCoroutine( enumerator));
        }

        IEnumerator ParallelCoroutine( IEnumerator enumerator)
        {
            yield return Koroutine.Nested( enumerator);
            done = true;
        }

        public bool HasDone()
        {
            return done;
        }

        public void OnYield( ICoroutineEngine engine)
        {
            engine.RegisterCustomYield( this);
        }

        public void Update( Method method)
        {

        }
    }
}
