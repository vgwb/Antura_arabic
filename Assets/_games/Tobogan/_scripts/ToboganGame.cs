namespace EA4S.Tobogan
{
    public class ToboganGame : TemplateGame
    {
        ToboganIntroductionState introductionState;

        protected override void OnInitialize(IGameContext context, int difficulty)
        {
            introductionState = new ToboganIntroductionState(this);
        }

        protected override IGameState GetInitialState()
        {
            return introductionState;
        }
    }
}