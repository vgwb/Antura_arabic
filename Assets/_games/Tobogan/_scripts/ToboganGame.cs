using UnityEngine;
using System.Collections;
using System;

namespace EA4S.Tobogan
{
    public class ToboganGame : TemplateGame
    {
        ToboganIntroductionState introductionState;
        
        public override void Initialize()
        {
            introductionState = new ToboganIntroductionState(this);
        }

        public override IGameState GetInitialState()
        {
            return introductionState;
        }
    }
}