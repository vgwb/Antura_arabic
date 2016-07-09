using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ModularFramework.Core;

namespace ModularFramework.Modules {
    public class OnClickShowUIContainer : MonoBehaviour {

        public string UIContainerName;
        OpenUIContainerSettings Settings = null;

        void Awake() {
            if (gameObject.GetComponent<Button>())
                gameObject.GetComponent<Button>().onClick.AddListener(OnMouseDown);
        }

        public void OnMouseDown() {
            GameManager.Instance.UIModule.ShowUIContainer(UIContainerName, Settings);
        }
    }
}