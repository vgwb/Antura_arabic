using UnityEngine;
using System.Collections.Generic;
namespace EA4S.MixedLetters
{
    public class IntroductionGameState : IGameState
    {
        MixedLettersGame game;

        float timer = 4;
        public IntroductionGameState(MixedLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timer = 4;
            game.GenerateNewWord();

            SeparateLettersSpawnerController.instance.SpawnLetters(game.lettersInOrder);
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.SetCurrentState(game.PlayState);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}