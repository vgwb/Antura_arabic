using System.Collections;
using UnityEngine;

namespace Antura.Core.Services.WebView
{
    public class WebViewService : MonoBehaviour
    {
        private GameObject webViewGameObject;
        private WebViewObject webViewObject;

        public void OpenUrl(string url)
        {
            StartCoroutine(InitWeb(cleanUrl(url)));
        }

        public void Close()
        {
            Destroy(webViewGameObject);
        }

        private IEnumerator InitWeb(string _url)
        {
            webViewGameObject = new GameObject("WebViewObject");
            webViewObject = webViewGameObject.AddComponent<WebViewObject>();
            webViewObject.Init(
                cb: (msg) => {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                },
                err: (msg) => {
                    Debug.Log(string.Format("CallOnError[{0}]", msg));
                },
                ld: (msg) => {
                    Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                    webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
                },
                //ua: "custom user agent string",
                enableWKWebView: true);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            webViewObject.bitmapRefreshCycle = 1;
#endif

            webViewObject.SetMargins(0, 300, 0, 0);
            webViewObject.SetVisibility(true);

            webViewObject.LoadURL(_url);

            yield break;
        }

        private string cleanUrl(string _url)
        {
            return _url.Replace(" ", "%20");
        }

    }
}