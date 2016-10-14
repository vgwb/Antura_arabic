using System;
using UnityEngine;

namespace EA4S
{
    public interface IPopupWidget
    {
        // Manual popup management
        void Show(bool reset = true);
        void SetButtonCallback(System.Action callback);
        void SetTitle(string text, bool isArabic);
        void SetTitle(TextID text);
        void SetImage(Sprite image);
        void SetWord(WordData data); // Modifies Text + Image      
        void SetMark(bool visible, bool correct);  
        void Hide();

        void ShowTimeUp(System.Action callback);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, TextID text, bool markResult, WordData word = null);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, TextID text, WordData word = null);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, string text, bool isArabic);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, Sprite image);
    }
}
