namespace EA4S.Assessment
{
    public class AssessmentIntroState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentIntroState(AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        float timer = 1;

        public void EnterState()
        {
            TimeEngine.Instance.Clear();
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                assessmentGame.SetCurrentState( assessmentGame.QuestionState);
            }
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}