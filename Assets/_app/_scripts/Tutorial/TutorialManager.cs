using Antura.Core;
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

        protected abstract AppScene CurrentAppScene { get; }

        public void HandleStart()
        {
            /*if (FirstContactManager.I.IsSequenceFinished()) {
                gameObject.SetActive(false);
                IsRunning = false;
                if (VERBOSE) { Debug.Log("TutorialManager - First contact is off"); }
                return;
            }*/

            if (VERBOSE) { Debug.Log("TutorialManager - phase " + FirstContactManager.I.CurrentPhaseInSequence + ""); }
            IsRunning = true;

            // Check what we already unlocked and enable / disable UI
            foreach (var phase in FirstContactManager.I.GetPhasesForScene(CurrentAppScene))
            {
                SetPhaseUIShown(phase, IsPhaseCompleted(phase));
            }

            // Check whether we are in a phase that this tutorial should handle
            if (!FirstContactManager.I.IsTutorialToBePlayed(CurrentAppScene))
            {
                StopTutorialRunning();
            }

            InternalHandleStart();
        }

        protected void StopTutorialRunning()
        {
            IsRunning = false;
        }

        protected void CompleteTutorialPhase()
        {
            StopTutorialRunning();
            FirstContactManager.I.CompleteCurrentPhaseInSequence();

            // Check if we have more
            HandleStart();
        }

        protected abstract void InternalHandleStart();

        protected virtual void SetPhaseUIShown(FirstContactPhase phase, bool choice)
        {
        }


        #region Phase Unlocking

        protected void AutoUnlockAndComplete(FirstContactPhase phase)
        {
            if (!FirstContactManager.I.HasCompletedPhase(phase))
            {
                FirstContactManager.I.CompletePhase(phase);
            }
        }

        protected bool IsPhaseUnlocked(FirstContactPhase phase)
        {
            return FirstContactManager.I.HasUnlockedPhase(phase);
        }

        protected bool IsPhaseCompleted(FirstContactPhase phase)
        {
            return FirstContactManager.I.HasCompletedPhase(phase);
        }

        protected bool IsPhaseToBeCompleted(FirstContactPhase phase)
        {
            return FirstContactManager.I.IsPhaseUnlockedAndNotCompleted(phase);
            /*bool shouldBeUnlocked = !FirstContactManager.I.HasCompletedPhase(phase) && unlockingCondition;
            if (shouldBeUnlocked) FirstContactManager.I.UnlockPhase(phase);
            return shouldBeUnlocked;*/
        }

        protected void UnlockPhaseIf(FirstContactPhase phase, bool unlockingCondition)
        {
            bool shouldBeUnlocked = !FirstContactManager.I.HasUnlockedPhase(phase) && unlockingCondition;
            if (shouldBeUnlocked) FirstContactManager.I.UnlockPhase(phase);
        }

        protected void UnlockPhaseIfReachedJourneyPosition(FirstContactPhase phase, int st, int lb, int ps)
        {
            bool shouldBeUnlocked = !FirstContactManager.I.HasUnlockedPhase(phase) && HasReachedJourneyPosition(st, lb, ps);
            if (shouldBeUnlocked) FirstContactManager.I.UnlockPhase(phase);
        }

        // Check for unlock
        protected bool HasReachedJourneyPosition(int st, int lb, int ps)
        {
            return AppManager.I.Player.MaxJourneyPosition.IsGreaterOrEqual(new JourneyPosition(st, lb, ps));
        }

        #endregion
    }
}