using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    public class LetterScaredState : LetterState
    {
        public float ScaredDuration = 1.0f;
        public Vector3 ScareSource;

        float scaredTimer;
        
        public LetterScaredState(FastCrowdLivingLetter letter) : base(letter)
        {

        }

        public override void EnterState()
        {
            scaredTimer = ScaredDuration;

            // set letter animation
            letter.gameObject.GetComponent<LetterObjectView>().Model.State = LetterObjectState.LL_run_fear;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            scaredTimer -= delta;

            if (scaredTimer <= 0)
                letter.SetCurrentState(letter.WalkingState);
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
