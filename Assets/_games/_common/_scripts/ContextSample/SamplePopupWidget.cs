using System;
using UnityEngine;

namespace EA4S
{
    public class SamplePopupWidget : IPopupWidget
    {
        public void Show(System.Action callback, TextID text, bool markResult, WordData word = null)
        {
            if (word != null)
                WidgetPopupWindow.I.ShowSentenceAndWordWithMark(callback, text.ToString(), word, markResult);
            else
                WidgetPopupWindow.I.ShowSentenceWithMark(callback, text.ToString(), markResult, null);
        }

        public void Show(System.Action callback, TextID text, WordData word = null)
        {
            if (word != null)
                WidgetPopupWindow.I.ShowSentenceAndWord(callback, text.ToString(), word);
            else
                WidgetPopupWindow.I.ShowSentence(callback, text.ToString());
        }

        public void Show(System.Action callback, string text, bool isArabic)
        {
            if (isArabic)
                WidgetPopupWindow.I.ShowArabicTextDirect(callback, text);
            else
                WidgetPopupWindow.I.ShowTextDirect(callback, text);
        }

        public void Show(Action callback, Sprite image)
        {
            WidgetPopupWindow.I.ShowTutorial(callback, image);
        }

        public void ShowTimeUp(Action callback)
        {
            WidgetPopupWindow.I.ShowTimeUp(callback);
        }

        public void Hide()
        {
            WidgetPopupWindow.I.Close();
        }
    }
}
