using System;
using EA4S.Core;
using EA4S.MinigamesCommon;

namespace EA4S.Assessment
{
    /// <summary>
    /// Result state. notify the LogManager of game ended and play final animation.
    /// Also teleport to main map.
    /// </summary>
    public class AssessmentResultState : IGameState
    {
        private AssessmentGame assessmentGame;
        private AssessmentDialogues dialogueManager;

        public AssessmentResultState( AssessmentGame assessmentGame, AssessmentDialogues dialogueManager)
        {
            this.assessmentGame = assessmentGame;
            this.dialogueManager = dialogueManager;
        }

        public void EnterState()
        {
            AssessmentConfiguration.Instance.Context.GetLogManager().OnGameEnded(3);

            var audioManager = assessmentGame.Context.GetAudioManager();

            audioManager.PlayMusic( Music.Relax);
            audioManager.PlaySound( Sfx.TickAndWin);
            dialogueManager.PlayAssessmentCompleteSound();

            AssessmentResultAntura.Instance.StartAnimation( //TODO: need new Antura Gag
                    () => ExitState()
                );
        }

        internal void InitState()
        {
            throw new NotImplementedException();
        }

        bool exited = false;
        public void ExitState()
        {
            if (exited == false) {
                NavigationManager.I.GoToScene( AppScene.Rewards);
                exited = true;
            }
        }

        public void Update( float delta)
        {

        }

        public void UpdatePhysics( float delta)
        {

        }
    }
}
