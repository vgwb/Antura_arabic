using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using System;

namespace EA4S.FastCrowd {

    public class WordComposer : MonoBehaviour {

        WordFlexibleContainer WordLabel;
        List<LL_LetterData> CompletedLetters = new List<LL_LetterData>();

        void Start() {
            WordLabel = GetComponent<WordFlexibleContainer>();
            UpdateWord();
        }
        
        void UpdateWord()
        {
            if (!isActiveAndEnabled)
                return;

            string word = string.Empty;
            foreach (LL_LetterData letter in CompletedLetters) {
                word += letter.Data.Isolated;
            }
            word = ArabicAlphabetHelper.ParseWord(word, AppManager.Instance.Teacher.GetAllTestLetterDataLL());
            WordLabel.SetText(word, false);
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
            transform.DOShakeScale(1.5f);
            UpdateWord();
        }

        private void DropContainer_OnObjectiveBlockCompleted() {
            Clean();
        }
    }
}