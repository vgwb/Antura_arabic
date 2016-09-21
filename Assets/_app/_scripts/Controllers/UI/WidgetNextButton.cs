using UnityEngine;
using System;
using System.Collections;

namespace EA4S
{
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

            AudioManager.I.PlaySfx(Sfx.UIPopup);
            WidgetPanel.SetActive(true);
        }

        public void Close()
        {
            AudioManager.I.PlaySfx(Sfx.UIButtonClick);
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