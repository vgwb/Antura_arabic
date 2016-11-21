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

        public void ExitState()
        {

        }

        public void Update( float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                Debug.Log("Loaded app_Rewards");
                AppManager.Instance.Modules.SceneModule.LoadSceneWithTransition( "app_Rewards");
            }
        }

        public void UpdatePhysics( float delta)
        {
        }
    }
}
