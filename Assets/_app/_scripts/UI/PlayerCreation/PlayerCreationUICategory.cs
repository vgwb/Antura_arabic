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
                    foreach (UIButton uiButton in uiButtons) uiButton.Toggle(true);
                    DispatchOnDeselectAll(this);
                }
            }
            else
            {
                // Select index
                SelectedIndex = index;
                for (int i = 0; i < uiButtons.Length; ++i) uiButtons[i].Toggle(i == index);
            }
        }

        public void SetColor(Color color)
        {
            foreach (UIButton uiButton in uiButtons)
            {
                uiButton.DefaultColor = color;
                uiButton.BtImg.color = new Color(color.r, color.g, color.b, uiButton.BtImg.color.a);
            }
        }

        public void ResetColor()
        {
            foreach (UIButton uiButton in uiButtons)
            {
               uiButton.DefaultColor = Color.white;
               uiButton.BtImg.color = new Color(1, 1, 1, uiButton.BtImg.color.a);
            }
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