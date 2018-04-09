using Antura.Core;
using UnityEngine;

namespace Antura.Kiosk
{

    public class WebPanel : MonoBehaviour
    {

        public void Open(string url)
        {
            gameObject.SetActive(true);
            AppManager.I.Services.WebView.OpenUrl(url);
        }

        public void OnClose()
        {

        }
    }
}