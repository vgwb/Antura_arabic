// Written by Davide Barbieri <davide.barbieri AT ghostshark.it>

using UnityEngine;

namespace EA4S
{
    public class SampleQuestionWidget : IQuestionWidget
    {
        public void Show(string text, WordData word, System.Action callback)
        {
            WidgetPopupWindow.I.ShowStringAndWord(callback, text, word);
        }

        public void Hide()
        {
            WidgetPopupWindow.I.Close();
        }
    }
}
