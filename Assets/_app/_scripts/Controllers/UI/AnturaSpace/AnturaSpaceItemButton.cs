// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/25

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class AnturaSpaceItemButton : UIButton
    {
        RenderTexture renderTexture;

        #region Unity

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (renderTexture != null) {
                renderTexture.Release();
                renderTexture.DiscardContents();
            }
        }

        #endregion

        #region Public Methods

        public void Setup()
        {
            // Create and assing new RenderTexture
            renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            this.GetComponentInChildren<Camera>(true).targetTexture = renderTexture;
            this.GetComponentInChildren<RawImage>(true).texture = renderTexture;
        }

        #endregion
    }
}