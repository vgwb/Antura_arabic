using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S
{
    public class StrollingLetterFallingState : StrollingLetterState
    {
        LetterObjectView view;
        float fallSpeed = 0;

        public StrollingLetterFallingState(StrollingLivingLetter letter) : base(letter)
        {
            view = letter.gameObject.GetComponent<LetterObjectView>();
        }

        public override void EnterState()
        {
            fallSpeed = 0;

            // set letter animation
            view.Falling = true;
        }

        public override void ExitState()
        {
            view.Falling = false;
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
