

using UnityEngine;

namespace EA4S.UI
{
    public class PlayerCreationUIPalette : PlayerCreationUICategory
    {
        #region Unity

        protected override void Awake()
        {
            base.Awake();

            // Set colors
            for (int i = 0; i < uiButtons.Length; ++i)
            {
                UIButton bt = uiButtons[i];
                Color color = PlayerColorConverter.ToColor((PlayerColor)(i + 1));
                bt.BtImg.color = bt.DefaultColor = color;
            }
        }

        #endregion
    }
}