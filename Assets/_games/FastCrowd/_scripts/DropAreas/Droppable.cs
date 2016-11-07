using UnityEngine;
using System.Collections;

namespace EA4S {
    /// <summary>
    /// Add functionality to be droppable on DropSingleArea.
    /// </summary>
    [RequireComponent(typeof(LetterObjectView))]
    [RequireComponent(typeof(Collider))]
    public class Droppable : MonoBehaviour
    {
        
        DropSingleArea dropAreaActive;
        
        
        public delegate void DropEvent(LetterObjectView _letterView);
        
        void OnTriggerEnter(Collider other) {
            DropSingleArea da = other.GetComponent<DropSingleArea>();
            if (da) {
                dropAreaActive = da;

            }
        }

        void OnTriggerExit(Collider other) {
            DropSingleArea da = other.GetComponent<DropSingleArea>();
            if (da && da == dropAreaActive) {
                dropAreaActive.DeactivateMatching();
                dropAreaActive = null;
            }
        }

    }
}