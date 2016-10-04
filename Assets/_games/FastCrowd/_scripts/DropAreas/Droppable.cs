using UnityEngine;
using System.Collections;
using Panda;

namespace EA4S {
    /// <summary>
    /// Add functionality to be droppable on DropSingleArea.
    /// </summary>
    [RequireComponent(typeof(LetterObjectView))]
    [RequireComponent(typeof(Collider))]
    public class Droppable : MonoBehaviour {

        LetterObjectView thisLetterView;

        DropSingleArea dropAreaActive;
        /// <summary>
        /// If dropAreaActive != null rappreset matching state with letter or word.
        /// </summary>
        [Task]
        public bool IsMatching {
            get { return isMatching; }
            set {
                isMatching = value;
            }
        }
        private bool isMatching = false;


        void Start() {
            thisLetterView = GetComponent<LetterObjectView>();
        }

        #region Tasks
        /// <summary>
        /// True if any DropSingleArea in collision.
        /// </summary>
        /// <returns></returns>
        [Task]
        public bool IsCollidingWithArea() {
            return dropAreaActive;
        }

        /// <summary>
        /// Set Match State preview on acrive colliding DropArea.
        /// </summary>
        /// <param name="_state"></param>
        [Task]
        public void SetMatchState(bool _state) {
            if (!dropAreaActive)
                return;
            if (_state)
                dropAreaActive.SetMatching();
            else
                dropAreaActive.SetMatchingWrong(); 
        }

        /// <summary>
        /// Unset Match State preview to prev state.
        /// </summary>
        [Task]
        public void UnSetMatchState() {
            dropAreaActive.DeactivateMatching();
        }

        [Task]
        public void Release() {
            GetComponent<Hangable>().ToBeRelease = false;
            if (isMatching) {
                if (OnRightMatch != null && IsCollidingWithArea())
                    OnRightMatch(thisLetterView);
            } else {
                if (OnWrongMatch != null && IsCollidingWithArea())
                    OnWrongMatch(thisLetterView);
            }

            Task.current.Succeed();
        }
        #endregion

        #region events
        public delegate void DropEvent(LetterObjectView _letterView);

        /// <summary>
        /// Happens whene wrong letter/word is dropped on active market.
        /// </summary>
        public static event DropEvent OnWrongMatch;

        /// <summary>
        /// Happens whene wrong letter/word is dropped on active market.
        /// </summary>
        public static event DropEvent OnRightMatch;
        #endregion

        #region trigger
        void OnTriggerEnter(Collider other) {
            DropSingleArea da = other.GetComponent<DropSingleArea>();
            if (da) {
                dropAreaActive = da;
                IsMatching = thisLetterView.Model.Data.Key == da.Data.Key;

            }
        }

        void OnTriggerExit(Collider other) {
            DropSingleArea da = other.GetComponent<DropSingleArea>();
            if (da && da == dropAreaActive) {
                dropAreaActive.DeactivateMatching();
                dropAreaActive = null;
                IsMatching = false;
            }
        }
        #endregion

    }
}