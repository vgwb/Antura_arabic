using System;
using DG.DeExtensions;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.UI
{
    /// <summary>
    /// Player creation category
    /// </summary>
    public class PlayerCreationUICategory : MonoBehaviour
    {
        #region Events

        public event Action<PlayerCreationUICategory,UIButton> OnSelect;
        public event Action<PlayerCreationUICategory> OnDeselectAll;
        void DispatchOnSelect(PlayerCreationUICategory category, UIButton uiButton) { if (OnSelect != null) OnSelect(category, uiButton); }
        void DispatchOnDeselectAll(PlayerCreationUICategory category) { if (OnDeselectAll != null) OnDeselectAll(category); }

        #endregion

        #region Serialized

        public PlayerCreationUI.CategoryType CategoryType;

        #endregion

        /// <summary>If nothing is selected, returns -1</summary>
        public int SelectedIndex { get; private set; }
        protected UIButton[] uiButtons;

        #region Unity

        protected virtual void Awake()
        {
            SelectedIndex = -1;
            uiButtons = this.GetComponentsInChildren<UIButton>();
            foreach (UIButton uiButton in uiButtons)
            {
                UIButton bt = uiButton;
                bt.Bt.onClick.AddListener(()=> OnClick(bt));
            }

            switch (CategoryType)
            {
                case PlayerCreationUI.CategoryType.Color:
                    // Set colors
                    for (int i = 0; i < uiButtons.Length; ++i)
                    {
                        UIButton bt = uiButtons[i];
                        Color color = PlayerTintConverter.ToColor((PlayerTint)(i + 1));
                        bt.ChangeDefaultColors(color, color);
                    }
                    break;
            }
        }

        void OnDestroy()
        {
            foreach (UIButton uiButton in uiButtons) uiButton.Bt.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public Methods

        // If index is less than 0 toggles all
        public void Select(int index)
        {
            if (index > uiButtons.Length - 1)
            {
                Debug.LogWarning("PlayerCreationUICategory.Select > Index out of range (captured)");
                return;
            }

            if (index < 0)
            {
                // Deselect all
                if (SelectedIndex >= 0)
                {
                    SelectedIndex = -1;
                    foreach (UIButton uiButton in uiButtons)
                    {
                        uiButton.Toggle(true);
                        if (CategoryType == PlayerCreationUI.CategoryType.Color) uiButton.transform.localScale = Vector3.one;
                    }
                    DispatchOnDeselectAll(this);
                }
            }
            else
            {
                // Select index
                SelectedIndex = index;
                for (int i = 0; i < uiButtons.Length; ++i)
                {
                    UIButton bt = uiButtons[i];
                    bt.Toggle(i == index);
                    if (CategoryType == PlayerCreationUI.CategoryType.Color) bt.transform.localScale = Vector3.one * (i == index ? 1 : 0.75f);
                }
            }
        }

        public void SetColor(Color color)
        {
            foreach (UIButton uiButton in uiButtons) uiButton.ChangeDefaultColors(color);
        }

        public void ResetColor()
        {
            foreach (UIButton uiButton in uiButtons) uiButton.ChangeDefaultColors(Color.white);
        }

        // Only used by avatars category
        public void AvatarSetIcon(bool isFemale)
        {
            // TODO use different avatars
            Sprite sprite = Resources.Load<Sprite>(AppConstants.AvatarsResourcesDir + (isFemale ? "F1" : "M1"));
            foreach (UIButton uiButton in uiButtons) uiButton.Ico.sprite = sprite;
        }

        #endregion

        #region Callbacks

        void OnClick(UIButton bt)
        {
            if (bt.IsToggled && SelectedIndex >= 0) Select(-1);
            else
            {
                int index = Array.IndexOf(uiButtons, bt);
                Select(index);
                DispatchOnSelect(this, uiButtons[index]);
            }
        }

        #endregion
    }
}