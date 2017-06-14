using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S.Core
{

    public class OnClickShowUIContainer : MonoBehaviour {

        public string UIContainerName;
        OpenUIContainerSettings Settings = null;

        void Awake() {
            if (gameObject.GetComponent<Button>())
                gameObject.GetComponent<Button>().onClick.AddListener(OnMouseDown);
        }

        public void OnMouseDown() {
            AppManager.Instance.UIModule.ShowUIContainer(UIContainerName, Settings);
        }
    }
}