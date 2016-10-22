using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    public class LetterWalkingState : LetterState
    {
        FastCrowdLetterMovement movement;
        
        public LetterWalkingState(FastCrowdLivingLetter letter) : base(letter)
        {
            movement = letter.GetComponent<FastCrowdLetterMovement>();

        }

        const float RUN_SPEED = 6.0f;
        const float WALK_SPEED = 3.0f;

        const float STUCK_THRESHOLD = 0.5f;

        float timer;
        Vector3 target;
        float speed;

        float stuckTimer;
        Vector3 lastPosition;

        public override void EnterState()
        {
            bool running = UnityEngine.Random.value < 0.5f;

            if (running)
            {
                // set letter animation
                letter.gameObject.GetComponent<LetterObjectView>().Model.State = LLAnimationStates.LL_run_happy;
                speed = RUN_SPEED;
            }
            else
            {

                // set letter animation
                letter.gameObject.GetComponent<LetterObjectView>().Model.State = LLAnimationStates.LL_walk;
                speed = WALK_SPEED;
            }

            timer = 3.0f + 5.0f * UnityEngine.Random.value;

            // Get a Random destination
            target = letter.walkableArea.GetRandomPosition();
            lastPosition = letter.transform.position;
            stuckTimer = STUCK_THRESHOLD;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            Vector3 distance = target - letter.transform.position;
            distance.y = 0;

            movement.MoveAmount(distance.normalized * speed * delta);
            movement.LerpLookAt(target, 4 * delta);

            timer -= delta;

            if (timer < 0 || distance.sqrMagnitude < 0.05f)
            {
                letter.SetCurrentState(letter.IdleState);
            }

            // if stuck for too long, change direction
            float avgSpeed = Vector3.Distance(letter.transform.position, lastPosition)/delta;

            if (avgSpeed < 0.5f * speed)
            {
                stuckTimer -= Time.deltaTime;
            }
            else
                stuckTimer = STUCK_THRESHOLD;

            if (stuckTimer <= 0)
            {
                // change direction
                target = letter.crowd.walkableArea.GetNearestPoint(letter.transform.position - distance);
                stuckTimer = STUCK_THRESHOLD;
            }

            lastPosition = letter.transform.position;

#if UNITY_EDITOR
            Debug.DrawLine(letter.transform.position, target);
#endif
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
