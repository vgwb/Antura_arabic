using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using TMPro;
using EA4S;

namespace EA4S.MakeFriends
{
    public class LetterChoiceController : MonoBehaviour
    {
        public TMP_Text LetterLabel;
        public Animator animator;

        [HideInInspector]
        public LetterData letterData;

        public enum ChoiceState
        {
            IDLE,
            CORRECT,
            WRONG
        }
        private ChoiceState _state;
        public ChoiceState State
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
            letterData = _letterData;
            LetterLabel.text = ArabicAlphabetHelper.GetLetterFromUnicode(letterData.Isolated_Unicode);
        }

        public void ClickAction()
        {
            MakeFriendsGameManager.Instance.OnClickedLetterChoice(this);
            SpeakLetter();
        }

        public void SpeakLetter()
        {
            if (letterData != null && letterData.Key != null)
            {
               AudioManager.I.PlayLetter(letterData.Key);
            }
        }

        public void FlashWrong()
        {
            animator.SetTrigger("FlashWrong");
        }

        private void OnStateChanged()
        {
            switch (State)
            {
                case ChoiceState.IDLE:
                    animator.SetTrigger("Idle");
                    break;
                case ChoiceState.CORRECT:
                    animator.SetTrigger("Correct");
                    break;
                case ChoiceState.WRONG:
                    animator.SetTrigger("Wrong");
                    break;
                default:
                    break;
            }
        }
    }
}