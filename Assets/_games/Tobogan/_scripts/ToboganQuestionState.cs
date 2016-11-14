namespace EA4S.Tobogan
{
    public class ToboganQuestionState : IGameState
    {
        ToboganGame game;

        float timer;
        public ToboganQuestionState(ToboganGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.questionsManager.Initialize();
            timer = 2;

            game.Context.GetAudioManager().PlayMusic(Music.MainTheme);
        }

        public void ExitState() { }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                if (game.showTutorial)
                {
                    game.SetCurrentState(game.TutorialState);
                }
                else
                {
                    game.SetCurrentState(game.PlayState);
                }
                return;
            }
        }

        public void UpdatePhysics(float delta) { }
    }
}