namespace EA4S
{
    /// <summary>
    /// Concrete implementation of IStarsWidget. Accessible to minigames.
    /// </summary>
    public class MinigamesStarsWidget : IStarsWidget
    {
        public void Show(int stars)
        {
            GameResultUI.ShowEndgameResult(stars);
        }

        public void Hide()
        {
            GameResultUI.HideEndgameResult();
        }
    }
}
