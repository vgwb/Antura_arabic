namespace EA4S.FastCrowd
{
    public class FastCrowdIntroductionState : IGameState
    {
        FastCrowdGame game;

        float timer = 1;
        public FastCrowdIntroductionState(FastCrowdGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetAudioManager().PlayMusic(Music.Theme3);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.QuestionState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}