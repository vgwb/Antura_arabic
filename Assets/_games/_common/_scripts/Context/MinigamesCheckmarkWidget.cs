using Antura.UI;

namespace Antura.MinigamesCommon
{
    /// <summary>
    /// Concrete implementation of ICheckmarkWidget. Accessible to minigames.
    /// </summary>
    public class MinigamesCheckmarkWidget : ICheckmarkWidget
    {
        public void Show(bool correct)
        {
            GlobalUI.I.ActionFeedback.Show(correct);
        }
    }
}
