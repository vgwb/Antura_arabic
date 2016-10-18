using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    public class LetterFallingState : LetterState
    {
        public LetterFallingState(FastCrowdLivingLetter letter) : base(letter)
        {

        }

        public override void EnterState()
        {
            // set letter animation
            letter.gameObject.GetComponent<LetterObjectView>().Model.State = LetterObjectState.LL_fall_down;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
