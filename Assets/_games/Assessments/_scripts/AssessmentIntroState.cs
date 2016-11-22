namespace EA4S.Assessment
{
    public class AssessmentIntroState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentIntroState(AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        AssessmentAnturaController anturaController;

        public void EnterState()
        {
            assessmentGame.Context.GetAudioManager().PlayMusic( Music.Theme7);
            TimeEngine.Instance.Clear();
        }

        public void ExitState()
        {
        }

        private void SetNextState()
        {
            assessmentGame
                   .SetCurrentState(
                   assessmentGame.PlayState);
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
