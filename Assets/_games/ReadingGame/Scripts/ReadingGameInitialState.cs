using System;
using UnityEngine;

namespace EA4S.ReadingGame
{
    public class ReadingGameInitialState : IGameState
    {
        ReadingGameGame game;

        float timer = 2;

        public ReadingGameInitialState(ReadingGameGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timer = 2;
        }


        public void ExitState()
        {
           
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
                game.SetCurrentState(game.ReadState);
        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}