namespace EA4S.MakeFriends
{
    public class MakeFriendsIntroductionState : IGameState
    {
        MakeFriendsGame game;

        float timer = 1;
        bool playTutorial = true;

        public MakeFriendsIntroductionState(MakeFriendsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            if (playTutorial)
            {
                this.game.PlayTutorial();
            }
            else
            {
                game.SetCurrentState(game.QuestionState);
            }
        }

        public void OnFinishedTutorial()
        {
            game.SetCurrentState(game.QuestionState);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
//            timer -= delta;
//            if (timer < 0)
//            {
//                game.SetCurrentState(game.QuestionState);
//            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}