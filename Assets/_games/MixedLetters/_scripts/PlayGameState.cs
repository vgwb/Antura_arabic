using UnityEngine;

namespace EA4S.MixedLetters
{
    public class PlayGameState : IGameState
    {
        public static bool RoundWon = false;
        MixedLettersGame game;

        float timer = 4;
        public PlayGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            RoundWon = false;
            game.ShowDropZones();
            SeparateLettersSpawnerController.instance.SetLettersDraggable(true);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            //timer -= delta;

            if (timer < 0 || RoundWon)
            {
                game.SetCurrentState(game.ResultState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
