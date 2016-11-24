using UnityEngine;

namespace EA4S.FastCrowd
{
    public class StrollingLetterTutorialState : StrollingLetterState
    {
        LetterCharacterController movement;

        Vector3 target;

        public StrollingLetterTutorialState(StrollingLivingLetter letter) : base(letter)
        {
            movement = letter.GetComponent<LetterCharacterController>();
        }

        const float RUN_SPEED = 6.0f;

        float speed;

        bool onTarget;

        public override void EnterState()
        {
            target = letter.walkableArea.tutorialPosition.position;

            // Set letter animation            
            Vector3 distance = target - letter.transform.position;
            distance.y = 0;

            if (distance.sqrMagnitude < 0.05f)
            {
                letter.gameObject.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_idle);
            }
            else
            {
                bool running = !letter.walkableArea.IsInside(letter.transform.position, true);

                // set letter animation
                letter.gameObject.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_walking);
                letter.gameObject.GetComponent<LetterObjectView>().SetWalkingSpeed(LetterObjectView.RUN_SPEED);
                speed = RUN_SPEED;
            }
        }

        public override void ExitState()
        {
            letter.gameObject.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_idle);
        }

        public override void Update(float delta)
        {
            Vector3 distance = target - letter.transform.position;
            distance.y = 0;

            if (distance.sqrMagnitude < 0.05f)
            {
                // Face Camera!
                movement.LerpLookAt(Camera.main.transform.position, 8 * delta);
            }
            else
            {
                movement.MoveAmount(distance.normalized * speed * delta);
                movement.LerpLookAt(target, 4 * delta);

                distance = target - letter.transform.position;
                distance.y = 0;

                if (distance.sqrMagnitude < 0.05f)
                {
                    letter.gameObject.GetComponent<LetterObjectView>().SetState(LLAnimationStates.LL_idle);
                }
            }
        }

        public override void UpdatePhysics(float delta) { }
    }
}
