namespace SRDebugger.UI.Other
{
    using Internal;
    using SRF;
    using UnityEngine;

    [RequireComponent(typeof (Canvas))]
    public class ConfigureCanvasFromSettings : SRMonoBehaviour
    {
        private void Start()
        {
            var c = GetComponent<Canvas>();

            SRDebuggerUtil.ConfigureCanvas(c);

            Destroy(this);
        }
    }
}
