using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentResultState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentResultState( AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        float timer = 0.7f;

        public void EnterState()
        {
            assessmentGame.Context.GetAudioManager().PlayMusic( Music.Relax);
        }


        public void ExitState()
        {
        }

        public void Update( float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                Debug.Log( "WARNING: SetEnd Game... needed SetEndAssessment"); // someone will implement that
                assessmentGame.EndGame( 0, 0);
            }
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}
