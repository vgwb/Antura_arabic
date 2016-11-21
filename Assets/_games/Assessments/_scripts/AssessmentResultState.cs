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

        float timer = 4f;

        public void EnterState()
        {
            assessmentGame.Context.GetAudioManager().PlayMusic( Music.Relax);
            assessmentGame.Context.GetAudioManager().PlaySound( Sfx.TickAndWin);
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
            timer -= delta;

            if (timer < 0)
                ExitState();
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}
