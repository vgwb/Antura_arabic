// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/04 11:47
// License Copyright (c) Daniele Giardini

using DG.DeExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public enum MenuButtonType
    {
        Unset,
        PauseToggle,
        Continue,
        Back,
        MusicToggle,
        FxToggle,
        Restart,
        Credits
    }

    public class MenuButton : MonoBehaviour
    {
        public MenuButtonType Type;

        public bool IsToggled { get; private set; }
        public Button Bt { get { if (fooBt == null) fooBt = this.GetComponent<Button>(); return fooBt; } }
        public Image Ico { get { if (fooIco == null) fooIco = this.GetOnlyComponentsInChildren<Image>(true)[0]; return fooIco; } }

        Button fooBt;
        Image fooIco;

        public void Toggle(bool activate)
        {
            Ico.SetAlpha(activate ? 1 : 0.4f);
        }
    }
}