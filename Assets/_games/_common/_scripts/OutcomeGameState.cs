using UnityEngine;
using System.Collections;
using System;

namespace EA4S
{
    public class OutcomeGameState : IGameState
    {
        MiniGame game;

        public OutcomeGameState(MiniGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            game.Context.GetStarsWidget().Show(game.StarsScore);
        }

        public void ExitState()
        {
            game.Context.GetStarsWidget().Hide();
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
