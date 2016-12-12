namespace EA4S.Assessment
{
    public class AssessmentIntroState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentIntroState( AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

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

        float timer = 0.4f;

        public void Update( float delta)
        {
            TimeEngine.Instance.Update( delta);
            timer -= delta;
            if (timer <= 0)
            {
                SetNextState();
            }
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}
