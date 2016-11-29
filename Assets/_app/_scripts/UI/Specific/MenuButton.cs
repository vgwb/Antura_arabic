// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/04 11:47

using UnityEngine;
using UnityEngine.UI;
using DG.DeExtensions;
using DG.Tweening;

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

    public class MenuButton : UIButton
    {
        public MenuButtonType Type;
    }
}
