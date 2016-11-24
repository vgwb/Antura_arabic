using UnityEngine;

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
                    return AssessmentFactory.CreateLetterInWordAssessment();

                case AssessmentCode.LetterShape:
                    return AssessmentFactory.CreateLetterShapeAssessment();

                case AssessmentCode.WordsWithLetter:
                    return AssessmentFactory.CreateWordsWithLetterAssessment();
            }
            return null;
        }

        public void EnterState()
        {
            Debug.Log(" assessment:" + assessment);
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
