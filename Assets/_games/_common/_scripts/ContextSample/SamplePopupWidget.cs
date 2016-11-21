using ArabicSupport;
using System;
using UnityEngine;

namespace EA4S
{
    public class SamplePopupWidget : IPopupWidget
    {
        public void Show(System.Action callback, TextID text, bool markResult, LL_WordData word = null)
        {
            if (word != null)
                WidgetPopupWindow.I.ShowSentenceAndWordWithMark(callback, text.ToString(), word, markResult);
            else
                WidgetPopupWindow.I.ShowSentenceWithMark(callback, text.ToString(), markResult, null);
        }

        public void Show(System.Action callback, TextID text, LL_WordData word = null)
        {
            if (word != null)
                WidgetPopupWindow.I.ShowSentenceAndWord(callback, text.ToString(), word);
            else
                WidgetPopupWindow.I.ShowSentence(callback, text.ToString());
        }

        public void Show(System.Action callback, string text, bool markResult, LL_WordData word = null)
        {
            if (word != null)
                WidgetPopupWindow.I.ShowSentenceAndWordWithMark(callback, text, word, markResult);
            else
                WidgetPopupWindow.I.ShowSentenceWithMark(callback, text, markResult, null);
        }

        public void Show(System.Action callback, string text, LL_WordData word = null)
        {
            if (word != null)
                WidgetPopupWindow.I.ShowSentenceAndWord(callback, text, word);
            else
                WidgetPopupWindow.I.ShowSentence(callback, text);
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

        public void Show(bool reset = true)
        {
            if (reset)
                WidgetPopupWindow.I.ResetContents();

            WidgetPopupWindow.I.Show(true);
        }

        public void SetButtonCallback(System.Action callback)
        {
            WidgetPopupWindow.I.SetButtonCallback(callback);
        }

        public void SetTitle(string text, bool isArabic)
        {
            WidgetPopupWindow.I.SetTitle(text, isArabic);
        }

        public void SetMessage(string text, bool isArabic)
        {
            WidgetPopupWindow.I.SetMessage(text, isArabic);
        }

        public void SetTitle(TextID text)
        {
            WidgetPopupWindow.I.SetTitleSentence(text.ToString());
        }

        public void SetMark(bool visible, bool correct)
        {
            WidgetPopupWindow.I.SetMark(visible, correct);
        }

        public void SetImage(Sprite image)
        {
            WidgetPopupWindow.I.SetImage(image);
        }

        public void SetWord(LL_WordData data)
        {
            WidgetPopupWindow.I.SetWord(data.Id, ((LL_WordData)data).Data.Arabic);
        }
    }
}
