using Antura.Core;
using UnityEngine;
using TMPro;

namespace Antura.UI
{
    /// <summary>
    /// Shows the version of the application. Used in the Home scene.
    /// </summary>
    public class VersionText : MonoBehaviour
    {
        void Start()
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = "v " + AppConstants.AppVersion;
        }
    }
}