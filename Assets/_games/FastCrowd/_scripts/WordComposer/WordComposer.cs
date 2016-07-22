using UnityEngine;
using System.Collections.Generic;

namespace EA4S
{

    public class WordComposer : MonoBehaviour
    {

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
        public void UpdateWord() {
            string word = string.Empty;
            foreach (LetterData letter in CompletedLetters) {
                word += letter.Isolated;
            }
            word = ArabicAlphabetHelper.ParseWord(word, AppManager.Instance.Letters);
            WordLabel.SetText(word, false);
        }

        #endregion

        #region event subscription delegates

        private void Droppable_OnRightMatch(LetterObjectView _letterView) {
            CompletedLetters.Add(_letterView.Model.Data);
            UpdateWord();
        }

        private void DropContainer_OnObjectiveBlockCompleted() {
            CompletedLetters = new List<LetterData>();
            UpdateWord();
        }

        #endregion

        #region event subscription

        void OnEnable() {
            DropContainer.OnObjectiveBlockCompleted += DropContainer_OnObjectiveBlockCompleted;
            Droppable.OnRightMatch += Droppable_OnRightMatch;
        }

        void OnDisable() {
            DropContainer.OnObjectiveBlockCompleted -= DropContainer_OnObjectiveBlockCompleted;
            Droppable.OnRightMatch -= Droppable_OnRightMatch;
        }

        #endregion
    }
}