using EA4S.Audio;

namespace EA4S.Minigames.DancingDots
{
    public class ResultGameState : IState
    {
        DancingDotsGame game;

        float timer = 1.5f;

        public ResultGameState(DancingDotsGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
			AudioManager.I.PlayMusic(Music.Relax);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
				game.EndGame(game.currStarsNum, game.numberOfRoundsWon);

                if (game.currStarsNum == 0)
                    AudioManager.I.PlayDialogue("Reward_0Star");
                else
                    AudioManager.I.PlayDialogue("Reward_" + game.currStarsNum + "Star_" + UnityEngine.Random.Range(1, 4));
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}