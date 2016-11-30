using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using System;

namespace EA4S.FastCrowd
{

    public class WordComposer : MonoBehaviour
    {
        public Transform innerTransform;
        WordFlexibleContainer WordLabel;
        List<LL_LetterData> CompletedLetters = new List<LL_LetterData>();

        void Awake()
        {
            WordLabel = GetComponent<WordFlexibleContainer>();
            UpdateWord();
        }

        void UpdateWord()
        {
            if (!isActiveAndEnabled)
                return;

            string word = string.Empty;

            for (int i = 0; i < CompletedLetters.Count; ++i) {
                LL_LetterData letter = CompletedLetters[i];
                word += letter.Data.GetChar();
            }
            
            WordLabel.SetText(word, true);
        }

        public void AddLetter(ILivingLetterData data)
        {
            if (!isActiveAndEnabled)
                return;

            StartCoroutine(AddLetter(data, 1.3f));
        }

        public void Clean()
        {
            CompletedLetters = new List<LL_LetterData>();
            UpdateWord();

            StopAllCoroutines();
        }

        IEnumerator AddLetter(ILivingLetterData data, float _delay)
        {
            yield return new WaitForSeconds(_delay);
            CompletedLetters.Add(data as LL_LetterData);
            AudioManager.I.PlaySfx(EA4S.Sfx.Hit);
            innerTransform.DOShakeScale(1.5f, 0.5f);
            UpdateWord();
        }

        private void DropContainer_OnObjectiveBlockCompleted()
        {
            Clean();
        }
    }
}