namespace EA4S.FastCrowd
{
    public class LetterIdleState : LetterState
    {
        public LetterIdleState(FastCrowdLivingLetter letter) : base(letter)
        {

        }

        float timer = 0.0f;
        public override void EnterState()
        {
            // Set letter animation
            letter.gameObject.GetComponent<LetterObjectView>().Model.State =
                //    (LetterObjectState)UnityEngine.Random.Range((int)LetterObjectState.LL_idle, (int)LetterObjectState.LL_idle_5);
                LetterObjectState.LL_idle_1;

            // Wait a random time in idle
            timer = 2.0f + 3.0f*UnityEngine.Random.value;
        }

        public override void ExitState()
        {
        }

        public override void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                letter.SetCurrentState(letter.WalkingState);
            }
        }

        public override void UpdatePhysics(float delta)
        {
        }
    }
}
