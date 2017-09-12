using Antura.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Antura.Core.Services.Gallery
{
    public class TakePhotoButton : MonoBehaviour, IPointerClickHandler
    {
        public TextRender PhotoCounterText;
        private int PhotosAvailable;

        void Start()
        {
            Init(10);
        }

        public void Init(int howManyPhotos)
        {
            gameObject.SetActive(true);
            PhotosAvailable = howManyPhotos;
            UpdateCounter();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Take Photo button!");
            if (PhotosAvailable > 0) {
                GetComponent<Button>().interactable = false;
                StartCoroutine(TakeScreenshot());
            }
        }

        private void PhotoFinished()
        {
            PhotosAvailable--;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            PhotoCounterText.SetText(PhotosAvailable.ToString());
            if (PhotosAvailable > 0) {
                GetComponent<Button>().interactable = true;
            } else {
                gameObject.SetActive(false);
            }
        }

        private IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();

            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            AppManager.I.Services.Gallery.SaveScreenshot(texture);
            PhotoFinished();
        }
    }
}
