using UnityEngine;

namespace EA4S.Assessment
{
    public class PausableWait : CustomYieldInstruction, ITickable
    {
        public PausableWait( float timeToWait)
        {
            timeRemaining = timeToWait;
        }

        private float timeRemaining;
        private bool elapsed = false;
        public override bool keepWaiting
        {
            get
            {
                if ( timeRemaining > 0)
                    return true;

                elapsed = true;
                return false;
            }
        }

        public bool Update( float deltaTime)
        {
            timeRemaining -= deltaTime;
            return elapsed;
        }
    }
}
