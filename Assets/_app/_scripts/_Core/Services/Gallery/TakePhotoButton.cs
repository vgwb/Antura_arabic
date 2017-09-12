using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Antura.Core.Services.Gallery
{
    public class TakePhotoButton : MonoBehaviour, IPointerClickHandler
    {

        void Start()
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Take Photo button!!");
            StartCoroutine(TakeScreenshot());
        }

        private IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();

            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            AppManager.I.Services.Gallery.SaveScreenshot(texture);
        }
    }
}
