// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/25

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class AnturaSpaceItemButton : UIButton
    {
        public GameObject IcoLock;
        public GameObject IcoNew;
        public Transform RewardContainer;

        [System.NonSerialized] public RewardItem Data;
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

        public void Lock(bool _doLock)
        {
            IcoLock.SetActive(_doLock);
            Bt.interactable = !_doLock;
        }

        public void SetAsNew(bool _isNew)
        {
            IcoNew.SetActive(_isNew);
        }

        #endregion
    }
}