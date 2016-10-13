using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using Google2u;
using TMPro;

namespace EA4S.MakeFriends
{
    public class LetterChoiceController : MonoBehaviour
    {
        public TMP_Text LetterLabel;
        public Animator animator;
        public Image image;
        public Button button;

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
            Reset();
            letterData = _letterData;
            LetterLabel.text = ArabicAlphabetHelper.GetLetterFromUnicode(letterData.Isolated_Unicode);
        }

        public void ClickAction()
        {
            Disable();
            SpeakLetter();
            MakeFriendsGameManager.Instance.OnClickedLetterChoice(this);
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

        public void SpawnBalloon(bool correctChoice)
        {
            var balloon = Instantiate(MakeFriendsGameManager.Instance.letterBalloonPrefab, MakeFriendsGameManager.Instance.letterBalloonContainer.transform.position, Quaternion.identity, MakeFriendsGameManager.Instance.letterBalloonContainer.transform) as GameObject;
            var balloonController = balloon.GetComponent<LetterBalloonController>();
            balloonController.Init(letterData);
            balloonController.EnterScene(correctChoice);
        }

        private void Disable()
        {
            image.enabled = false;
            button.enabled = false;
            LetterLabel.enabled = false;
        }

        private void Reset()
        {
            image.enabled = true;
            button.enabled = true;
            LetterLabel.enabled = true;
            State = ChoiceState.IDLE;
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