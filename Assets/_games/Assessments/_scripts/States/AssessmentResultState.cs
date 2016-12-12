namespace EA4S.Assessment
{
    public class AssessmentResultState : IGameState
    {
        private AssessmentGame assessmentGame;

        public AssessmentResultState(AssessmentGame assessmentGame)
        {
            this.assessmentGame = assessmentGame;
        }

        public void EnterState()
        {

            AssessmentConfiguration.Instance.Context.GetLogManager().OnMiniGameResult(3);

            var audioManager = assessmentGame.Context.GetAudioManager();
            IDialogueManager dialogue = new DialogueManager(audioManager, assessmentGame.Context.GetSubtitleWidget());
            audioManager.PlayMusic(Music.Relax);
            audioManager.PlaySound(Sfx.TickAndWin);
            dialogue.Dialogue(Localization.Random(
                                                    Db.LocalizationDataId.Assessment_Complete_1,
                                                    Db.LocalizationDataId.Assessment_Complete_2,
                                                    Db.LocalizationDataId.Assessment_Complete_3), true);

            AssessmentResultAntura.Instance.StartAnimation(
                () => ExitState()
                );
        }

        bool exited = false;
        public void ExitState()
        {
            if (exited == false) {
                NavigationManager.I.GoToScene(AppScene.Rewards);
                exited = true;
            }
        }

        public void Update(float delta)
        {
            TimeEngine.Instance.Update(delta);
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
