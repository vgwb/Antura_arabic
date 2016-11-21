using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentIntroState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentIntroState(AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        float timer = 200;

        public void EnterState()
        {
            assessmentGame.Context.GetAudioManager().PlayMusic( Music.Relax);
            TimeEngine.Instance.Clear();

            // A GameObject with TutorialHelper component is needed
            TutorialUI.ClickRepeat( TutorialHelper.GetWorldPosition());

            var antura = Object.Instantiate(assessmentGame.antura) as AssessmentAnturaController;
            antura.gameObject.SetActive(true);

            antura.SetFinishedAnimationCallback(() => SetNextState());
        }

        public void ExitState()
        {
        }

        private void SetNextState()
        {
            assessmentGame
                   .SetCurrentState(
                   assessmentGame.QuestionState);
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
