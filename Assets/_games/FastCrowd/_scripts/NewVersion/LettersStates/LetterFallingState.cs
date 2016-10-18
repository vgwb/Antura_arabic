using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    public class LetterFallingState : LetterState
    {
        LetterObjectView view;
        float fallSpeed = 0;

        public LetterFallingState(FastCrowdLivingLetter letter) : base(letter)
        {
            view = letter.gameObject.GetComponent<LetterObjectView>();
        }

        public override void EnterState()
        {
            fallSpeed = 0;

            // set letter animation
            view.Model.State = LetterObjectState.LL_fall_down;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            fallSpeed += Physics.gravity.y * delta;

            var currentPos = view.transform.position;

            currentPos.y += fallSpeed * delta;

            if (currentPos.y <= 0)
            {
                currentPos.y = 0;
                letter.SetCurrentState(letter.IdleState);
            }

            view.transform.position = currentPos;
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
