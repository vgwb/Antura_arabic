using UnityEngine;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class ResultGameState : IGameState
    {
        ColorTickleGame game;

        float timer = 0.5f;
        public ResultGameState(ColorTickleGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            Debug.Log("Result State activated");
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                //AudioManager.I.PlayDialog(TextID.GetTextIDFromStars(game.starsAwarded).ToString());
                game.EndGame(game.starsAwarded,0);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
