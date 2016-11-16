using UnityEngine;
using System.Collections;
using TMPro;
using EA4S;

public class VersionText : MonoBehaviour
{

    void Start()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = "v " + AppConstants.AppVersion;
    }
}
