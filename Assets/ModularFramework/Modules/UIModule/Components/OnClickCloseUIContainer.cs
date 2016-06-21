using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ModularFramework.Core;
using UniRx;

namespace ModularFramework.Modules {
    public class OnClickCloseUIContainer : MonoBehaviour {

        //public string UIContainerName;
        OpenUIContainerSettings Settings;

        void OnEnable() {
            Button button = gameObject.GetComponent<Button>();
            if (button)
                button.onClick.AsObservable().Subscribe(_ => {
                    OnMouseDown();
                }).AddTo(this);
        }

        public void OnMouseDown() {
            UIContainer container = transform.GetComponentInParent<UIContainer>();
            GameManager.Instance.UIModule.HideUIContainer(container.Key);
        }
    }
}
