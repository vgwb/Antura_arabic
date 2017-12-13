namespace Antura.Rewards
{
    /// <summary>
    /// Structure focused to comunicate about colors from e to UI.
    /// </summary>
    /// <seealso cref="RewardColor" />
    public class RewardColorItem : RewardColor  // reward color for customization & UI
    {
        public bool IsSelected;
        public bool IsNew = true;

        public RewardColorItem()
        {
        }

        public RewardColorItem(RewardColor _color)
        {
            ID = _color.ID;
            Color1Name = _color.Color1Name;
            Color1RGB = _color.Color1RGB;
            Color2Name = _color.Color2Name;
            Color2RGB = _color.Color2RGB;
        }
    }
}