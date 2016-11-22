using System.Collections;
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

        AssessmentAnturaController anturaController;

        public void EnterState()
        {
            assessmentGame.Context.GetAudioManager().PlayMusic( Music.Theme7);
            TimeEngine.Instance.Clear();

            // A GameObject with TutorialHelper component is needed
            Coroutine.Start( TutorialClicks());

            anturaController = AnturaFactory.Instance.SleepingAntura();

            anturaController.SetFinishedAnimationCallback( () => SetNextState());
        }

        private IEnumerator TutorialClicks()
        {
            yield return TimeEngine.Wait( 0.6f);
            TutorialUI.ClickRepeat( TutorialHelper.GetWorldPosition());
            yield return TimeEngine.Wait( 0.1f);

            for (int i = 0; i < 9; i++)
            {
                yield return TimeEngine.Wait( 0.17f);
                assessmentGame.Context.GetAudioManager().PlaySound( Sfx.ThrowObj);
            }

            yield return TimeEngine.Wait( 0.1f);
            anturaController.EnableClick();
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
