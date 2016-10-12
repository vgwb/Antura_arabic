using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;

namespace EA4S.MakeFriends
{
    public class LetterPickerController : MonoBehaviour
    {
        public LetterChoiceController[] letterChoices;
        public Animator animator;
        public GameObject letterPickerBlocker;

        [HideInInspector]
        public List<LetterChoiceController> IdleLetterPrompts
        {
            get { return new List<LetterChoiceController>(letterChoices).FindAll(choice => choice.isActiveAndEnabled && choice.State == LetterChoiceController.ChoiceState.IDLE); }
        }


        public void DisplayLetters(List<LetterData> letters)
        {
            for (int i = 0; i < letters.Count; i++)
            {
                letterChoices[i].gameObject.SetActive(true);
                letterChoices[i].Init(letters[i]);
            }
        }

        public void Reset()
        {
            Block();

            foreach (var prompt in letterChoices)
            {
                prompt.State = LetterChoiceController.ChoiceState.IDLE;
                prompt.gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            animator.SetTrigger("Entrance");
        }

        public void ShowAndUnblockDelayed(float delay)
        {
            StopCoroutine("ShowAndUnblockDelayed_Coroutine");
            StartCoroutine("ShowAndUnblockDelayed_Coroutine", delay);
        }

        private IEnumerator ShowAndUnblockDelayed_Coroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Show();
            Unblock();
        }

        public void Hide()
        {
            animator.SetTrigger("Exit");
        }

        public void Block()
        {
            letterPickerBlocker.SetActive(true);
        }

        public void Unblock()
        {
            letterPickerBlocker.SetActive(false);
        }
    }
}
