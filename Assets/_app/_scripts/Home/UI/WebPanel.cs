using Antura.Core;
using Antura.Core.Services.WebView;
using UnityEngine;

namespace Antura.Kiosk
{

    public class WebPanel : MonoBehaviour
    {

        public void Open(string url)
        {
            gameObject.SetActive(true);
            WebViewComponent.I.OpenBrowser(url, 0, 100, 0, 0);
        }

        public void OnClose()
        {
            WebViewComponent.I.CloseBrowser();
            gameObject.SetActive(false);
        }
    }
}