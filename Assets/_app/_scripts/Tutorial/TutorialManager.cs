using Antura.Profile;
using UnityEngine;

namespace Antura.Tutorial
{
    /// <summary>
    /// Base class for tutorial managers inside scenes.
    /// </summary>
    public abstract class TutorialManager : MonoBehaviour
    {
        public static bool VERBOSE = true;
        public bool IsRunning { get; protected set; }

        public void HandleStart()
        {
            if (FirstContactManager.I.IsSequenceFinished()) {
                gameObject.SetActive(false);
                IsRunning = false;
                if (VERBOSE) { Debug.Log("TutorialManager - First contact is off"); }
                return;
            }

            if (VERBOSE) { Debug.Log("TutorialManager - phase " + FirstContactManager.I.CurrentPhaseInSequence + ""); }
            IsRunning = true;   // TODO: if we are  not in a correct step, set as false!

            InternalHandleStart();
        }

        protected void CompleteTutorialPhase()
        {
            IsRunning = false;
            FirstContactManager.I.CompleteCurrentPhaseInSequence();

            // Check if we have more
            HandleStart();
        }

        protected abstract void InternalHandleStart();
    }
}