namespace EA4S.Assessment
{
    public class AssessmentResultState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentResultState( AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        float timer = 4f;

        public void EnterState()
        {
            assessmentGame.Context.GetAudioManager().PlayMusic( Music.Relax);
            assessmentGame.Context.GetAudioManager().PlaySound( Sfx.TickAndWin);
            AssessmentResultAntura.Instance .StartAnimation(
                () => ExitState()
                );
        }

        bool exited = false;
        public void ExitState()
        {
            if (exited == false)
            {
                AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition("app_Rewards");
                exited = true;
            }
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
