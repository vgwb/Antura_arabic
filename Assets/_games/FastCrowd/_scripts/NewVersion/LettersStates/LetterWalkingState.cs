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

        const float RUN_SPEED = 6.0f;
        const float WALK_SPEED = 3.0f;

        float timer;
        Vector3 target;
        float speed;

        public override void EnterState()
        {
            bool running = UnityEngine.Random.value < 0.5f;

            if (running)
            {
                // set letter animation
                letter.gameObject.GetComponent<LetterObjectView>().Model.State = LetterObjectState.LL_run_happy;
                speed = RUN_SPEED;
            }
            else
            {

                // set letter animation
                letter.gameObject.GetComponent<LetterObjectView>().Model.State = LetterObjectState.LL_walk;
                speed = WALK_SPEED;
            }

            timer = 3.0f + 5.0f * UnityEngine.Random.value;

            // Get a Random destination
            target = letter.walkableArea.GetRandomPosition();
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            Vector3 distance = target - letter.transform.position;
            letter.transform.position += distance.normalized * speed * delta;
            letter.LerpLookAt(target, 4 * delta);

            timer -= delta;

            if (timer < 0 || distance.sqrMagnitude < 0.05f)
            {
                letter.SetCurrentState(letter.IdleState);
            }
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
