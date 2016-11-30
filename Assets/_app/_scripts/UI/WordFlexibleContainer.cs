using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;
using EA4S;
using System;

public class WordFlexibleContainer : MonoBehaviour
{
    public TextRender Label;

    public void SetText(string text, bool arabic)
    {
        Label.setText(text, arabic);
    }

    public void SetText(ILivingLetterData data)
    {
        Label.SetLetterData(data);
    }

    public void Reset()
    {
        Label.text = "";
    }

    internal void SetNumber(int numberValue)
    {
        Label.setText(numberValue.ToString(), false);
    }
}
