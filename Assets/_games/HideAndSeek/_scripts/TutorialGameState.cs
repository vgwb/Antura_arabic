namespace EA4S.HideAndSeek
{
    public class TutorialGameState : IGameState
    {
        HideAndSeekGame game;

        public TutorialGameState(HideAndSeekGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            //game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
            AudioManager.I.PlayMusic(Music.Lullaby);
            game.TutorialManager.enabled = true;
        }

        public void ExitState()
        {
            game.TutorialManager.enabled = false;
            //game.Context.GetAudioManager().StopMusic();
            AudioManager.I.StopMusic();
        }

        public void Update(float delta) { }

        public void UpdatePhysics(float delta) { }
    }
}
