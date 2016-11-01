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
            game.Context.GetStarsBarWidget().Hide();

            int starsScore = game.StarsScore;
            if (starsScore > 3)
                starsScore = 3;

            game.Context.GetStarsWidget().Show(starsScore);

            var subTitleWidget = game.Context.GetSubtitleWidget();
            subTitleWidget.DisplaySentence(TextID.GetTextIDFromStars(starsScore));
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
