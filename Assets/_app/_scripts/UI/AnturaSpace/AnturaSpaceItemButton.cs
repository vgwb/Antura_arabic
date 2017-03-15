using EA4S.Rewards;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.UI
{
    /// <summary>
    /// Button for an item in a category in the Antura Space scene.
    /// </summary>
    public class AnturaSpaceItemButton : UIButton
    {
        public RawImage RewardImage;
        public GameObject IcoLock;
        public GameObject IcoNew;
        public Camera RewardCamera;
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

        public override void Lock(bool _doLock)
        {
            base.Lock(_doLock);

            IcoLock.SetActive(_doLock);
            RewardImage.gameObject.SetActive(!_doLock);
            if (_doLock) IcoNew.SetActive(false);
        }

        public void SetAsNew(bool _isNew)
        {
            IcoNew.SetActive(_isNew);
        }

        public void SetImage(bool _isRenderTexture)
        {
            RewardImage.texture = _isRenderTexture ? renderTexture : null;
        }

        #endregion
    }
}