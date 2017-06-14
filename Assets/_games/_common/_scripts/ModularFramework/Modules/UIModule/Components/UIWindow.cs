
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace EA4S.Core
{

    public abstract class UIWindow : MonoBehaviour {

        public UIWindowTypes Type;

        RectTransform rectTransform;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            Type = SetWindowType();
        }
        
        /// <summary>
        /// Force set window type for any UIWindow derived classes.
        /// </summary>
        /// <returns></returns>
        public abstract UIWindowTypes SetWindowType();

        public void Show() {
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Hide() {
            rectTransform.anchoredPosition = new Vector2(0, Screen.height);
        }
    }
}