using System;

namespace EA4S.Tobogan
{
    public class ToboganGame : MiniGame
    {
        public ToboganIntroductionState IntroductionState { get; private set; }
        public ToboganPlayState PlayState { get; private set; }

        protected override IGameConfiguration GetConfiguration()
        {
            return ToboganConfiguration.Instance;
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }

        protected override void OnInitialize(IGameContext context)
        {
            IntroductionState = new ToboganIntroductionState(this);
            PlayState = new ToboganPlayState(this);
        }
    }
}