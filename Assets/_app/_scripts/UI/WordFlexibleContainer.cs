using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;
using EA4S;

public class WordFlexibleContainer : MonoBehaviour
{
    public TextRender Label;

    public void SetText(string text)
    {
       Label.text = text;
    }

    public void Reset()
    {
        Label.text = "";
    }

}
