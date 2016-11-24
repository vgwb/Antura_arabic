using System;
using UnityEngine;

namespace EA4S
{
    public interface IPopupWidget
    {
        // Manual popup management
        void Show(bool reset = true);
        void SetButtonCallback(System.Action callback);
        void SetTitle(Db.LocalizationDataId text);
        void SetTitle(string text);
        void SetMessage(Db.LocalizationDataId text);
        void SetMessage(string text);
        void SetImage(Sprite image);
        void SetLetterData(ILivingLetterData data); // Modifies Text + Image      
        void SetMark(bool visible, bool correct);  
        void Hide();

        void ShowTimeUp(System.Action callback);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, Db.LocalizationDataId text, bool markResult, LL_WordData word = null);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, Db.LocalizationDataId text, LL_WordData word = null);

        [Obsolete("Using manual configuration", false)]
        void Show(System.Action callback, Sprite image);
    }
}
