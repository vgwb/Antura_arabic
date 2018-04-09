using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Core.Services.WebView
{
    public class WebViewService : MonoBehaviour
    {
        public string Url;
        public Text StatusText;
        private GameObject webViewGameObject;
        private WebViewObject webViewObject;

        public void OnOpen()
        {
            StartCoroutine(InitWeb());
        }

        public void OnClose()
        {
            Destroy(webViewGameObject);
        }

        IEnumerator InitWeb()
        {
            webViewGameObject = new GameObject("WebViewObject");
            webViewObject = webViewGameObject.AddComponent<WebViewObject>();
            webViewObject.Init(
                cb: (msg) => {
                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
                    StatusText.text = msg;
                },
                err: (msg) => {
                    Debug.Log(string.Format("CallOnError[{0}]", msg));
                    StatusText.text = msg;
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

            if (Url.StartsWith("http")) {
                webViewObject.LoadURL(Url.Replace(" ", "%20"));
            }

            yield break;
        }

    }
}