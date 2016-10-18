namespace EA4S.FastCrowd
{
    public class LetterHangingState : LetterWalkingState
    {
        public LetterHangingState(FastCrowdLivingLetter letter) : base(letter)
        {

        }

        public override void EnterState()
        {
            base.EnterState();

            // set letter animation
            letter.gameObject.GetComponent<LetterObjectView>().Model.State = LetterObjectState.LL_drag_idle;
        }
    }
}
