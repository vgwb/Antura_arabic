using UnityEngine;
using System.Collections;

namespace EA4S { 

    public class WordComposer : MonoBehaviour {

	    // Use this for initialization
	    void Start () {
	
	    }
	
	    // Update is called once per frame
	    void Update () {
	
	    }

        #region API
        /// <summary>
        /// 
        /// </summary>
        public void UpdateWord() {

        }

        #endregion

        #region event subscription delegates

        private void Droppable_OnRightMatch(LetterObjectView _letterView) {
            UpdateWord();
        }

        private void DropContainer_OnObjectiveBlockCompleted() {
            throw new System.NotImplementedException();
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