namespace EA4S.FastCrowd
{
    public abstract class LetterState : IGameState
    {
        protected FastCrowdLivingLetter letter;

        public LetterState(FastCrowdLivingLetter letter)
        {
            this.letter = letter;
        }

        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void Update(float delta);
        public abstract void UpdatePhysics(float delta);
    }
}
