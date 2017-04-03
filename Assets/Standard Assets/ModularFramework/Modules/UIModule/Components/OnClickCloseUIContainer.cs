using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ModularFramework.Core;

namespace ModularFramework.Modules {
    public class OnClickCloseUIContainer : MonoBehaviour {

        void OnEnable() {
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }

        public void OnMouseDown() {
            UIContainer container = transform.GetComponentInParent<UIContainer>();
            GameManager.Instance.UIModule.HideUIContainer(container.Key);
        }
    }
}
