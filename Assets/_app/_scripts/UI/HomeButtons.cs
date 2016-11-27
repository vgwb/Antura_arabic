// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/04 12:49
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace EA4S
{
    public class HomeButtons : MonoBehaviour
    {
        public HomeManager HomeMngr;
        public Credits CreditsWindow;
        public MenuButton BtPlay, BtMusic, BtFx, BtCredits;

        MenuButton[] menuBts;

        void Start()
        {
            menuBts = this.GetComponentsInChildren<MenuButton>(true);
            foreach (MenuButton bt in menuBts) {
                MenuButton b = bt;
                b.Bt.onClick.AddListener(() => OnClick(b));
            }

            BtMusic.Toggle(AudioManager.I.MusicEnabled);
            BtFx.Toggle(AppManager.I.GameSettings.HighQualityGfx);
        }

        void OnDestroy()
        {
            foreach (MenuButton bt in menuBts)
                bt.Bt.onClick.RemoveAllListeners();
        }

        void OnClick(MenuButton bt)
        {
            switch (bt.Type) {
                case MenuButtonType.MusicToggle: // Music on/off
                    AudioManager.I.ToggleMusic();
                    BtMusic.Toggle(AudioManager.I.MusicEnabled);
                    break;
                case MenuButtonType.FxToggle: // FX on/off
                    AppManager.I.ToggleQualitygfx();
                    BtFx.Toggle(AppManager.I.GameSettings.HighQualityGfx);
                    break;
                case MenuButtonType.Continue:
                    HomeMngr.Play();
                    break;
                case MenuButtonType.Credits:
                    CreditsWindow.Open();
                    break;
            }
        }
    }
}