using UnityEngine;
using System.Collections;
using System;

namespace EA4S.Tobogan
{
    public class QuestionGameState : IGameState
    {
        TemplateGame game;

        public QuestionGameState(TemplateGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
        }

        public void UpdatePhysics(float delta)
        {
        }
    }
}
