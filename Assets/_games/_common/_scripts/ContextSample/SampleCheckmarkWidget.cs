using UnityEngine;

namespace EA4S
{
    public class SampleCheckmarkWidget : ICheckmarkWidget
    {
        public void Show(bool correct)
        {
            GlobalUI.I.ActionFeedback.Show(correct);
        }
    }
}
