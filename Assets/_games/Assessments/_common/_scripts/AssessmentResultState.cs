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
                //TODO: create method EndAssessment (don't need to show result screen)
                assessmentGame.EndGame( 0, 0);
            }
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}