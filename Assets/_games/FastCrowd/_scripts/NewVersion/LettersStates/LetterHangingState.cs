using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.FastCrowd
{
    public class LetterHangingState : LetterState
    {
        List<FastCrowdLivingLetter> near = new List<FastCrowdLivingLetter>();

        FastCrowdLetterMovement movement;

        public LetterHangingState(FastCrowdLivingLetter letter) : base(letter)
        {
            movement = letter.GetComponent<FastCrowdLetterMovement>();
        }

        public override void EnterState()
        {

            // set letter animation
            letter.gameObject.GetComponent<LetterObjectView>().Model.State = LLAnimationStates.LL_drag_idle;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            // Scare neighbourhood
            near.Clear();
            letter.crowd.GetNearLetters(near, letter.transform.position, 5.0f);
            for (int i = 0, count = near.Count; i < count; ++i)
                near[i].Scare(letter.transform.position, 2);

            // Face Camera!
            movement.LerpLookAt(Camera.main.transform.position, 8*delta);
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
