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

    public class MenuButton : MonoBehaviour
    {
        public MenuButtonType Type;
        public Color BtToggleOffColor = Color.white;

        public bool IsToggled { get; private set; }
        public Button Bt { get { if (fooBt == null) fooBt = this.GetComponent<Button>(); return fooBt; } }

        Button fooBt;
        Image btImg, ico;
        Color defColor;
        Tween clickTween;

        void Awake()
        {
            clickTween = this.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.35f).SetAutoKill(false).SetUpdate(true).Pause();
        }

        void OnDestroy()
        {
            clickTween.Kill();
        }

        public void Toggle(bool _activate)
        {
            if (btImg == null) {
                btImg = this.GetComponent<Image>();
                defColor = btImg.color;
            }
            if (ico == null) ico = this.GetOnlyComponentsInChildren<Image>(true)[0];

            btImg.color = _activate ? defColor : BtToggleOffColor;
            ico.SetAlpha(_activate ? 1 : 0.4f);
        }

        public void AnimateClick()
        {
            clickTween.Restart();
        }
    }
}
