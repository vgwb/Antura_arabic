namespace EA4S.Egg
{
    public class EggIntroductionState : IGameState
    {
        EggGame game;

        float timer = 1;
        public EggIntroductionState(EggGame game) { this.game = game; }

        public void EnterState()
        {
            //game.Context.GetSubtitleWidget().DisplaySentence(TextID.EGG_INTRO);
            game.antura.ResetAnturaIn(EggGame.numberOfStage, 2);
        }

        public void ExitState()
        {
            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.QuestionState);
                return;
            }
        }

        public void UpdatePhysics(float delta) { }
    }
}