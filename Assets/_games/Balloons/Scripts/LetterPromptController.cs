using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using TMPro;
using CGL.Antura;

public class LetterPromptController : MonoBehaviour
{
    public TMP_Text LetterLabel;
    public LetterData Data;
    //public DropContainer DropContain;

    //Vector3 enabledPos, disabledPos;

    public void Init(LetterData _letterData)
    {
        //DropContain = _dropContainer;
        //DropContain.Aree.Add(this);
        //enabledPos = transform.position;
        //disabledPos = enabledPos + new Vector3(0, -0.8f, 0);
        Data = _letterData;
        LetterLabel.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
    }


    public enum PromptState
    {
        IDLE,
        CORRECT,
        WRONG
    }

    private PromptState _state;
    public PromptState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                OnStateChanged();
            }
        }
    }

    void OnStateChanged()
    {
        switch (State)
        {
            case PromptState.IDLE:
                GetComponent<Renderer>().materials[0].color = Color.white;
                break;
            case PromptState.CORRECT:
                GetComponent<Renderer>().materials[0].color = Color.green;
                break;
            case PromptState.WRONG:
                GetComponent<Renderer>().materials[0].color = Color.red;
                break;
            default:
                break;
        }
    }


}

