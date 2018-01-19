using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Core.Services.Gallery
{

    public class GalleryManager : MonoBehaviour
    {
        public GameObject BtnTakePhoto;
        public GameObject PhotoFrame;
        public RawImage PhotoImage;

        public void ShowPreview(Texture texture)
        {
            PhotoImage.texture = texture;
            PhotoFrame.SetActive(true);
        }
        public void HidePreview()
        {
            PhotoFrame.SetActive(false);
        }
    }
}