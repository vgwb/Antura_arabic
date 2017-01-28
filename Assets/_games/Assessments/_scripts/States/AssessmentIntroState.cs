using EA4S.MinigamesCommon;

namespace EA4S.Assessment
{
    public class AssessmentIntroState : IState
    {
        private AssessmentGame assessmentGame;
        private AssessmentGameState gameState;
        private IAudioManager audioManager;

        public AssessmentIntroState(    AssessmentGame assessmentGame, 
                                        AssessmentGameState gameState,
                                        IAudioManager audioManager)
        {
            this.assessmentGame = assessmentGame;
            this.gameState = gameState;
            this.audioManager = audioManager;
        }

        public void InitAllStates()
        {

        }

        public void EnterState()
        {
            audioManager.PlayMusic( Music.Theme7);
        }

        public void ExitState()
        {
        }

        private void SetNextState()
        {
            assessmentGame.SetCurrentState( gameState);
        }

        float timer = 0.6f; // Gives Time to show the first question appearing

        public void Update( float delta)
        {
            timer -= delta;
            if (timer <= 0)
                SetNextState();
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}
