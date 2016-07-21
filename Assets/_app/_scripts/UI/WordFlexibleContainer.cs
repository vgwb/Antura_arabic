using UnityEngine;
using System.Collections;
using TMPro;

public class WordFlexibleContainer : MonoBehaviour
{

    public TextMeshProUGUI Label;

    void Start() {
	
    }

    public void SetText(string label) {
        Label.text = label;
    }

}
