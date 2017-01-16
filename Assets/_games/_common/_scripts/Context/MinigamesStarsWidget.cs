using UnityEngine;

namespace EA4S
{
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
