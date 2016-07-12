using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;

public class PopupWindowController : MonoBehaviour
{

    public static PopupWindowController I;
    public GameObject TitleGO;
    public GameObject DrawingContainerGO;
    public GameObject DrawingImageGO;
    public GameObject WordContainerGO;
    public GameObject WordTextGO;
    public GameObject ButtonGO;

    void Start() {
        I = this;
    }

    public void SetTitle(string text) {
        TitleGO.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
    }

    public void SetWord(string word) {
        // here set both word and drawing 
    }

    public void OnPressButton() {
        
    }
}
