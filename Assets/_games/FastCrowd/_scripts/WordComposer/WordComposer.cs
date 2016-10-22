using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using System;

namespace EA4S.FastCrowd {

    public class WordComposer : MonoBehaviour {

        WordFlexibleContainer WordLabel;
        List<LetterData> CompletedLetters = new List<LetterData>();

        void Start() {
            WordLabel = GetComponent<WordFlexibleContainer>();
            UpdateWord();
        }

        #region API

        /// <summary>
        /// 
        /// </summary>
        public void UpdateWord()
        {
            if (!isActiveAndEnabled)
                return;

            string word = string.Empty;
            foreach (LetterData letter in CompletedLetters) {
                word += letter.Isolated;
            }
            word = ArabicAlphabetHelper.ParseWord(word, AppManager.Instance.Letters);
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
            CompletedLetters = new List<LetterData>();
            UpdateWord();

            StopAllCoroutines();
        }
        
        #endregion

        #region event subscription delegates

        private void FastCrowd_OnReadyForGameplayDone(ModularFramework.Modules.IGameplayInfo _gameplayInfo) {
            // TODO: move to fastcrowd manager.
            // Disable this component for living words variant
            if (FastCrowdConfiguration.Instance.Variation == FastCrowdVariation.Words)
                gameObject.SetActive(false);
        }

        private void Droppable_OnRightMatch(LetterObjectView _letterView) {
            StartCoroutine(AddLetter(_letterView.Model.Data, 1.3f));
        }

        IEnumerator AddLetter(ILivingLetterData data, float _delay)
        {
            yield return new WaitForSeconds(_delay);
            CompletedLetters.Add(data as LetterData);
            AudioManager.I.PlaySfx(EA4S.Sfx.Hit);
            transform.DOShakeScale(1.5f);
            UpdateWord();
        }

        private void DropContainer_OnObjectiveBlockCompleted() {
            Clean();
        }

        #endregion

        #region event subscription

        void OnEnable() {
            DropContainer.OnObjectiveBlockCompleted += DropContainer_OnObjectiveBlockCompleted;
            Droppable.OnRightMatch += Droppable_OnRightMatch;
            FastCrowd.OnReadyForGameplayDone += FastCrowd_OnReadyForGameplayDone;
        }

        void OnDisable() {
            DropContainer.OnObjectiveBlockCompleted -= DropContainer_OnObjectiveBlockCompleted;
            Droppable.OnRightMatch -= Droppable_OnRightMatch;
            FastCrowd.OnReadyForGameplayDone -= FastCrowd_OnReadyForGameplayDone;
        }

        #endregion
    }
}