using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    public class LetterWalkingState : LetterState
    {
        public LetterWalkingState(FastCrowdLivingLetter letter) : base(letter)
        {

        }

        float timer;

        public override void EnterState()
        {
            // set letter animation
            letter.gameObject.GetComponent<LetterObjectView>().Model.State = LetterObjectState.LL_walk;

            timer = 3.0f + 5.0f * UnityEngine.Random.value;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                letter.SetCurrentState(letter.IdleState);
            }
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
