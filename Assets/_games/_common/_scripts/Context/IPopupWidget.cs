using UnityEngine;

namespace EA4S
{
    public interface IPopupWidget
    {
        void Show(System.Action callback, TextID text, bool markResult, WordData word = null);
        void Show(System.Action callback, TextID text,  WordData word = null);
        void Show(System.Action callback, string text, bool isArabic);
        void Show(System.Action callback, Sprite image);
        void ShowTimeUp(System.Action callback);

        void Hide();
    }
}
