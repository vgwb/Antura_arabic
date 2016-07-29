using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;

public class WordFlexibleContainer : MonoBehaviour
{

    public TextMeshProUGUI Label;

    void Start() {
	
    }

    public void SetText(string text, bool useArabicFixer) {
        if (useArabicFixer) {
            Label.text = ArabicFixer.Fix(text, false, false);
        } else {
            Label.text = text;
        }
    }

}
