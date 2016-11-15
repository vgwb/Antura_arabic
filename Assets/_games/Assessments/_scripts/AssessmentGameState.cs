namespace EA4S.Assessment
{
    public class AssessmentGameState : IGameState
    {
        private AssessmentGame assessmentGame;
        private IAssessment assessment;

        public AssessmentGameState( AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
            assessment = GetAssessment();
        }

        private IAssessment GetAssessment()
        {
            switch (AssessmentConfiguration.Instance.assessmentType)
            {
                case AssessmentCode.MatchLettersToWord:
                    return AssessmentFactory.CreateLetterInWordAssessment(
                        MatchLettersToWordConfiguration.Instance);

                case AssessmentCode.LetterShape:
                    return null;

                case AssessmentCode.WordsWithLetter:
                    return null;
            }
            return null;
        }

        public void EnterState()
        {
            Coroutine.Start( assessment.PlayCoroutine( SetNextState));
        }

        public void SetNextState()
        {
            assessmentGame.SetCurrentState( assessmentGame.ResultState);
        }

        public void ExitState()
        {

        }

        public void Update( float delta)
        {
            TimeEngine.Instance.Update( delta);
        }

        public void UpdatePhysics( float delta)
        {

        }
    }
}
