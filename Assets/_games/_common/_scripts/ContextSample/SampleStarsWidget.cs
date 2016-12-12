using UnityEngine;

namespace EA4S
{
    public class SampleStarsWidget : IStarsWidget
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
