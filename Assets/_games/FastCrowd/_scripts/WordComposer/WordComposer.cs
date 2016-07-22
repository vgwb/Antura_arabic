using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace EA4S { 

    public class WordComposer : MonoBehaviour {

        TextMeshProUGUI WordLable;
        List<LetterData> CompletedLetters = new List<LetterData>();

	    // Use this for initialization
	    void Start () {
            WordLable = GetComponent<WordFlexibleContainer>().Label;
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
            word = ArabicAlphabetHelper.ParseWord(word,AppManager.Instance.Letters);
            WordLable.text = word;
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