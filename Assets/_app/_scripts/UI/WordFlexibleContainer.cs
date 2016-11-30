using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;
using EA4S;
using System;

public class WordFlexibleContainer : MonoBehaviour
{
    public TextRender Label;
    public TextRender NumbersLabel;

    public void SetText(string text, bool arabic)
    {
        Label.gameObject.SetActive(true);
        NumbersLabel.gameObject.SetActive(false);
        Label.setText(text, arabic);
    }

    public void SetText(ILivingLetterData data)
    {
        Label.gameObject.SetActive(true);
        NumbersLabel.gameObject.SetActive(false);
        Label.SetLetterData(data);
    }

    public void Reset()
    {
        Label.gameObject.SetActive(true);
        NumbersLabel.gameObject.SetActive(false);
        Label.text = "";
        NumbersLabel.text = "";
    }

    public void SetNumber(int numberValue)
    {
        Label.gameObject.SetActive(false);
        NumbersLabel.gameObject.SetActive(true);
        NumbersLabel.setText(numberValue.ToString(), false);
    }
}
