using UnityEngine;
using System.Collections;
using TMPro;

public class SubtitlesController : MonoBehaviour
{
    public static SubtitlesController I;

    public GameObject Background;
    public GameObject TextGO;

    TextMeshProUGUI TextUI;

    void Awake() {
        I = this;
    }

    void Start() {
        TextUI = TextGO.GetComponent<TextMeshProUGUI>();
    }

    void OnEnable() {

    }

    public void DisplayText(string text) {
        if (text != "") {
            Background.SetActive(true);
            TextUI.text = text;
        } else {
            TextUI.text = "";
            Background.SetActive(false);
        }

    }
}
