using UnityEngine;
using System;
using EA4S.Audio;

namespace EA4S
{
    /// <summary>
    /// A general-purpose *Next* button.
    /// </summary>
    public class WidgetNextButton : MonoBehaviour
    {
        public static WidgetNextButton I;

        public GameObject WidgetPanel;
        Action currentCallback;

        void Awake()
        {
            I = this;
        }

        public void Show(Action callback)
        {
            currentCallback = callback;

            AudioManager.I.PlaySound(Sfx.UIPopup);
            WidgetPanel.SetActive(true);
        }

        public void Close()
        {
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            WidgetPanel.SetActive(false);
        }

        public void OnPressButton()
        {
            Close();
            if (currentCallback != null)
                currentCallback();
        }
    }
}