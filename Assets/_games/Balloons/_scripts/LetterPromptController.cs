using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using TMPro;
using EA4S;

namespace Balloons
{
    public class LetterPromptController : MonoBehaviour
    {
        public TMP_Text LetterLabel;
        public LetterData Data;
        public Animator animator;

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


        public void Init(LetterData _letterData)
        {
            Data = _letterData;
            LetterLabel.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
        }

        void OnStateChanged()
        {
            switch (State)
            {
                case PromptState.IDLE:
                    //GetComponent<Image>().color = Color.white;
                    animator.SetBool("Idle", true);
                    animator.SetBool("Correct", false);
                    animator.SetBool("Wrong", false);
                    break;
                case PromptState.CORRECT:
                    //GetComponent<Image>().color = Color.green;
                    animator.SetBool("Idle", false);
                    animator.SetBool("Correct", true);
                    animator.SetBool("Wrong", false);
                    break;
                case PromptState.WRONG:
                    //GetComponent<Image>().color = Color.red;
                    animator.SetBool("Idle", false);
                    animator.SetBool("Correct", false);
                    animator.SetBool("Wrong", true);
                    break;
                default:
                    break;
            }
        }
    }
}