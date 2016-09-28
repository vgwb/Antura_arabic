namespace EA4S.Tobogan
{
    public class ToboganGame : EA4S.Template.TemplateGame
    {
        /*
        public ToboganIntroductionState IntroductionState { get; private set; }
        public ToboganPlayState PlayState { get; private set; }
        
        protected override void OnInitialize(IGameContext context, int difficulty)
        {
            IntroductionState = new ToboganIntroductionState(this);
            PlayState = new ToboganPlayState(this);
        }

        protected override IGameState GetInitialState()
        {
            return IntroductionState;
        }
        */

        protected override void Start()
        {
            base.Start();

            // WARNING: THIS MUST BE CALLED BY THE GAME HUB OR WHAT CONFIGURES THE GAME
            Initialize(new SampleGameContext(), 0, new SampleWordProvider(5));
        }
    }
}