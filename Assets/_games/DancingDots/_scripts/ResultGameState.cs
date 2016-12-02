using System;
using UnityEngine;

namespace EA4S.DancingDots
{
    public class ResultGameState : IGameState
    {
        DancingDotsGame game;

        float timer = 2;

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
                    AudioManager.I.PlayDialog("Reward_0Star");
                else
                    AudioManager.I.PlayDialog("Reward_" + game.currStarsNum + "Star_" + UnityEngine.Random.Range(1, 4));
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}