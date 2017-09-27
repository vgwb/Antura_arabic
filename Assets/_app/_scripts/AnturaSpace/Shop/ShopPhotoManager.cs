using System;
using System.Collections;
using System.Collections.Generic;
using Antura.Audio;
using Antura.Core;
using Antura.Core.Services.Gallery;
using Antura.Debugging;
using Antura.Helpers;
using Antura.UI;
using Antura.Utilities;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopPhotoManager : SingletonMonoBehaviour<ShopPhotoManager>
    {
        public GalleryManager GalleryManager;

        public Action OnPhotoConfirmationRequested;
        public Action OnPurchaseCompleted;

        public List<GameObject> gameObjectsToHide = new List<GameObject>();

        [HideInInspector]
        public int CurrentPhotoCost;

        void Start()
        {
            gameObjectsToHide.Add(FindObjectOfType<DebugPanel>().gameObject);
            gameObjectsToHide.Add(FindObjectOfType<GlobalUI>().gameObject);
        }

        public void TakePhoto()
        {
            StartCoroutine(TakeScreenshotCO());
        }

        private Texture2D currentPhotoTexture;
        private IEnumerator TakeScreenshotCO()
        {
            AudioManager.I.PlaySound(Sfx.WheelTick);

            // Hide objects first
            foreach (var go in gameObjectsToHide)
                go.SetActive(false);

            yield return new WaitForEndOfFrame();

            Debug.Log("Taking the SCREEN");

            currentPhotoTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            currentPhotoTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            currentPhotoTexture.Apply();

            foreach (var go in gameObjectsToHide)
                go.SetActive(true);

            GalleryManager.ShowPreview(currentPhotoTexture);

            // Ask for confirmation
            ShopDecorationsManager.I.SetContextSpecialAction();
            if (OnPhotoConfirmationRequested != null) OnPhotoConfirmationRequested();
        }

        public void ConfirmPhoto()
        {
            Debug.Log("Saving the SCREEN");
            AppManager.I.Services.Gallery.SaveScreenshot(currentPhotoTexture);
            currentPhotoTexture = null;
            if (OnPurchaseCompleted != null) OnPurchaseCompleted();
            ShopDecorationsManager.I.SetContextPurchase();
            AppManager.I.Player.Save();

            GalleryManager.HidePreview();
        }

        public void CancelPhoto()
        {
            ShopDecorationsManager.I.SetContextPurchase();
            currentPhotoTexture = null;

            GalleryManager.HidePreview();
        }
    }
}