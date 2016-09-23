using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EA4S.Tobogan
{
    public class ToboganIntroductionState : IGameState
    {
        TemplateGame game;

        public ToboganIntroductionState(TemplateGame game)
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
