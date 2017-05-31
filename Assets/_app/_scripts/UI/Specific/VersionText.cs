using EA4S.Core;
using UnityEngine;
using TMPro;

namespace EA4S.UI
{
    /// <summary>
    /// Shows the version of the application. Used in the Home scene.
    /// </summary>
    public class VersionText : MonoBehaviour
    {
        void Start()
        {
            gameObject.GetComponent<TextMeshProUGUI_OLD>().text = "v " + AppConstants.AppVersion;
        }
    }
}