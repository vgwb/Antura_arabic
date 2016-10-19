using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    public class LetterScaredState : LetterState
    {
        public float ScaredDuration = 1.0f;
        public Vector3 ScareSource;

        const float SCARED_RUN_SPEED = 8.0f;

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
            // Run-away from danger!
            Vector3 runDirection = letter.transform.position - ScareSource;
            runDirection.y = 0;
            runDirection.Normalize();

            letter.transform.position += runDirection.normalized * SCARED_RUN_SPEED * delta;
            letter.LerpLookAt(letter.transform.position + runDirection, 4 * delta);

            if (Vector3.Distance(letter.transform.position, letter.antura.transform.position) < 25.0f)
            {
                letter.Scare(letter.antura.transform.position, 5);
                return;
            }

            if (Vector3.Distance(letter.transform.position, ScareSource) > 10.0f)
            {
                scaredTimer = Mathf.Min(1, scaredTimer);
            }

            scaredTimer -= delta;

            if (scaredTimer <= 0)
                letter.SetCurrentState(letter.WalkingState);
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
