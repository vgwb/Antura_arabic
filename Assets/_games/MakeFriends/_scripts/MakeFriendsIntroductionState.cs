using EA4S.MinigamesCommon;

namespace EA4S.Minigames.MakeFriends
{
    public class MakeFriendsIntroductionState : IState
    {
        MakeFriendsGame game;

        float timer = 1.5f;
        bool playTutorial = true;
        bool takenAction = false;

        public MakeFriendsIntroductionState(MakeFriendsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.PlayTitleVoiceOver();
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
            if (takenAction)
            {
                return;
            }

            timer -= delta;

            if (timer < 0)
            {
                takenAction = true;

                if (playTutorial)
                {
                    this.game.PlayTutorial();
                }
                else
                {
                    game.SetCurrentState(game.QuestionState);
                }
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}