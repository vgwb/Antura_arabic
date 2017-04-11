using UnityEngine;

namespace EA4S.Minigames.ColorTickle
{
    public class ResultGameState : IState
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
                game.EndGame(game.starsAwarded, game.score);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
