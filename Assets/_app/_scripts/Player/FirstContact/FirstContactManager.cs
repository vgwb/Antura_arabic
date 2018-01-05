using System.Collections.Generic;
using System.Linq;
using Antura.Core;
using UnityEngine;

namespace Antura.Profile
{
    /// <summary>
    /// Defines different phases for the First Contact.
    /// Each phase can be locked or unlocked. Locked phases are not shown.
    /// This system has two uses:
    /// - locking / unlocking of features. An unlocked feature has logic assigned to it during tutorial, until it is completed.
    /// - phase sequence: a set of phases that are unlocked one after the other. You cannot unlock the next one until you complete the previous one.
    /// @note: the order in the enum is not the sequence, the sequence is defined by the sequence list!
    /// </summary>
    public enum FirstContactPhase
    {
        Intro,

        Reward_FirstBig,

        Map_PlaySession,
        Map_GoToAnturaSpace,
        Map_GoToMinigames,
        Map_GoToBook,
        Map_GoToProfile,
        Map_CollectBones,

        AnturaSpace_TouchAntura,
        AnturaSpace_Customization,
        AnturaSpace_Exit,
        AnturaSpace_Shop,
        AnturaSpace_Photo,

        Finished,
        MAX,

        NONE
    }

    public enum FirstContactPhaseState
    {
        Locked = 0,
        Unlocked = 1,
        Completed = 2
    }

    [System.Serializable]
    public class FirstContactState
    {
        public FirstContactPhaseState[] phaseStates;

        public FirstContactState()
        {
            phaseStates = new FirstContactPhaseState[(int) FirstContactPhase.MAX];
        }

        public override string ToString()
        {
            string s = "";
            foreach (FirstContactPhase phase in System.Enum.GetValues(typeof(FirstContactPhase)))
            {
                if (phase == FirstContactPhase.MAX) continue;
                if (phase == FirstContactPhase.NONE) continue;
                s += phase + ": " + phaseStates[(int)phase] + "\n";
            }
            return s;
        }
    }

    /// <summary>
    /// Manages the flow of First Contact (i.e. the tutorial) throughout the application
    /// </summary>
    public class FirstContactManager
    {
        public static FirstContactManager I
        {
            get { return AppManager.I.FirstContactManager; }
        }

        // State
        private List<FirstContactPhase> phasesSequence;
        private FirstContactState state;

        // Debug
        public static bool VERBOSE = true;

        private static bool SIMULATE_FIRST_CONTACT = false;
        private FirstContactPhase SIMULATE_FIRST_CONTACT_PHASE = FirstContactPhase.AnturaSpace_TouchAntura;

        private static bool FORCE_FIRST_CONTACT_START = false;
        private FirstContactPhase FORCED_FIRST_CONTACT_START_PHASE = FirstContactPhase.Finished;

        public FirstContactManager()
        {
            if (!Application.isEditor) {
                // Force debug options to FALSE if we're not in the editor
                SIMULATE_FIRST_CONTACT = false;
                FORCE_FIRST_CONTACT_START = false;
            }

            // Define the first contact sequence
            // Phases in the sequence can be unlocked one after the other, so that when the previous one is completed we can start the next one
            phasesSequence = new List<FirstContactPhase>
            {
                FirstContactPhase.Intro,
                FirstContactPhase.Reward_FirstBig,

                FirstContactPhase.AnturaSpace_TouchAntura,
                FirstContactPhase.AnturaSpace_Customization,
                FirstContactPhase.AnturaSpace_Exit,

                FirstContactPhase.Map_PlaySession,

                //FirstContactPhase.Map_GoToAnturaSpace,

                FirstContactPhase.AnturaSpace_Shop,
                //FirstContactPhase.AnturaSpace_Photo,

                FirstContactPhase.Finished
            };

            // Default state
            state = new FirstContactState();
        }

        public void InitialiseForCurrentPlayer(FirstContactState _state)
        {
            this.state = _state;

            // Default state
            if (state == null || state.phaseStates.Length == 0)
            {
                state = new FirstContactState();
                ResetSequence();
                AppManager.I.Player.FirstContactState = state;
            }

            if (FORCE_FIRST_CONTACT_START)
            {
                ForceToPhaseInSequence(FORCED_FIRST_CONTACT_START_PHASE);
            }
        }

        #region Checks

        #region Sequence

        void ResetSequence()
        {
            phasesSequence.ForEach(phase => SetPhaseState(phase, FirstContactPhaseState.Locked));
            UnlockPhase(phasesSequence[0]);
        }

        public FirstContactPhase CurrentPhaseInSequence
        {
            get
            {
                if (SIMULATE_FIRST_CONTACT)
                {
                    return SIMULATE_FIRST_CONTACT_PHASE;
                }
                foreach (var phase in phasesSequence)
                {
                    if (!HasCompletedPhase(phase)) return phase;
                }
                return FirstContactPhase.Finished;
            }
        }

        public bool IsSequenceFinished()
        {
            if (AppConfig.DisableFirstContact) return true;
            if (SIMULATE_FIRST_CONTACT) return false;
            return phasesSequence.All(HasCompletedPhase);
        }

        public bool IsSequenceNotFinished()
        {
            return !IsSequenceFinished();
        }

        public void CompleteCurrentPhaseInSequence()
        {
            // Advance in the sequence
            var currentPhase = CurrentPhaseInSequence;
            CompletePhase(currentPhase);

            var nextPhase = phasesSequence[phasesSequence.IndexOf(currentPhase) + 1];
            UnlockPhase(nextPhase);
        }

        public void ForceToPhaseInSequence(FirstContactPhase forcedPhase)
        {
            ResetSequence();

            foreach (var phase in phasesSequence)
            {
                if (phase != forcedPhase)
                {
                    CompletePhase(phase);
                }
                else
                {
                    UnlockPhase(forcedPhase);
                    break;
                }
            }

            AppManager.I.Player.Save(); // TODO: save only when needed

            if (VERBOSE) Debug.Log("FirstContact - FORCING phase " + forcedPhase);
        }

        public void ForceToFinishedSequence()
        {
            ForceToPhaseInSequence(FirstContactPhase.Finished);
        }

        public void ForceToStartOfSequence()
        {
            ForceToPhaseInSequence(FirstContactPhase.Intro);
            AppManager.I.Player.ResetPlayerProfileCompletion();
        }

        #endregion

        #region Completion

        public void UnlockPhase(FirstContactPhase _phase)
        {
            SetPhaseState(_phase, FirstContactPhaseState.Unlocked);
            AppManager.I.Player.Save(); // TODO: save only when needed

            if (VERBOSE) Debug.Log("FirstContact - phase " + _phase + " unlocked!");
        }

        public void CompletePhase(FirstContactPhase _phase)
        {
            SetPhaseState(_phase, FirstContactPhaseState.Completed);
            AppManager.I.Player.Save(); // TODO: save only when needed

            if (VERBOSE) Debug.Log("FirstContact - phase " + _phase + " completed!");
        }

        public bool HasUnlockedPhase(FirstContactPhase _phase)
        {
            return GetPhaseState(_phase) == FirstContactPhaseState.Unlocked
                   || GetPhaseState(_phase) == FirstContactPhaseState.Completed;
        }

        public bool HasCompletedPhase(FirstContactPhase _phase)
        {
            return GetPhaseState(_phase) == FirstContactPhaseState.Completed;
        }

        public bool IsPhaseUnlockedAndNotCompleted(FirstContactPhase _phase)
        {
            return HasUnlockedPhase(_phase) && !HasCompletedPhase(_phase);
        }

        #endregion

        public override string ToString()
        {
            string s = "Next in sequence: " + CurrentPhaseInSequence;

            s += "\n\n-- Unlock state for sequence --\n";
            foreach (var phase in phasesSequence)
            {
                s += phase + ": " + GetPhaseState(phase) + "\n";
            }

            s += "\n-- Unlock state for other features --\n";
            for (int phase_i = 0; phase_i < state.phaseStates.Length; phase_i++)
            {
                var phase = (FirstContactPhase) phase_i;
                if (!phasesSequence.Contains(phase))
                {
                    s += phase + ": " + GetPhaseState(phase) + "\n";
                }
            }

            return s;
        }

        #region Internal

        private void SetPhaseState(FirstContactPhase _phase, FirstContactPhaseState _state)
        {
            state.phaseStates[(int)_phase] = _state;
        }

        private FirstContactPhaseState GetPhaseState(FirstContactPhase _phase)
        {
            return state.phaseStates[(int)_phase];
        }

        #endregion

        #endregion

        #region Navigation Filtering

        /// <summary>
        /// Filter the navigation from a scene to the next based on the first contact requirements
        /// </summary>
        public AppScene FilterNavigation(AppScene fromScene, AppScene toScene, out bool keepPrevAsBackable)
        {
            keepPrevAsBackable = false;
            if (IsSequenceFinished()) return toScene;

            // Check whether this transition is completing a phase
            TransitionCompletePhaseOn(FirstContactPhase.Intro, fromScene == AppScene.Intro);
            TransitionCompletePhaseOn(FirstContactPhase.AnturaSpace_Exit, fromScene == AppScene.AnturaSpace);
            TransitionCompletePhaseOn(FirstContactPhase.Map_PlaySession, fromScene == AppScene.PlaySessionResult);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToAnturaSpace, fromScene == AppScene.Map && toScene == AppScene.AnturaSpace);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToBook, fromScene == AppScene.Map && toScene == AppScene.Book);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToMinigames, fromScene == AppScene.Map && toScene == AppScene.Book);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToProfile, fromScene == AppScene.Map && toScene == AppScene.Book);

            // Check whether this transition needs to be filtered (i.e. it must go to a specific scene)
            // @note: we always filter if we are coming from home, to handle the fact that the player can shut down the game during a tutorial
            FilterTransitionOn(FirstContactPhase.Intro, fromScene == AppScene.Home && toScene == AppScene.Map, ref toScene, AppScene.Intro);
            FilterTransitionOn(FirstContactPhase.Reward_FirstBig, fromScene == AppScene.Home || fromScene == AppScene.Intro, ref toScene, AppScene.Rewards);
            FilterTransitionOn(FirstContactPhase.AnturaSpace_TouchAntura, fromScene == AppScene.Home || fromScene == AppScene.Rewards, ref toScene, AppScene.AnturaSpace);
            FilterTransitionOn(FirstContactPhase.AnturaSpace_Shop, fromScene == AppScene.Home || fromScene == AppScene.PlaySessionResult, ref toScene, AppScene.AnturaSpace);

            // Force the game to re-start from the current tutorial phase if needed
            FilterTransitionOn(FirstContactPhase.Map_GoToProfile, fromScene == AppScene.Home, ref toScene, AppScene.Map);
            FilterTransitionOn(FirstContactPhase.Map_GoToAnturaSpace, fromScene == AppScene.Home, ref toScene, AppScene.Map);
            FilterTransitionOn(FirstContactPhase.Map_GoToMinigames, fromScene == AppScene.Home, ref toScene, AppScene.Map);
            FilterTransitionOn(FirstContactPhase.Map_GoToBook, fromScene == AppScene.Home, ref toScene, AppScene.Map);

            return toScene;
        }

        private void FilterTransitionOn(FirstContactPhase phase, bool condition, ref AppScene toScene, AppScene newScene)
        {
            if (IsPhaseUnlockedAndNotCompleted(phase) && condition) { toScene = newScene; }
        }

        private void TransitionCompletePhaseOn(FirstContactPhase phase, bool condition)
        {
            if (IsPhaseUnlockedAndNotCompleted(phase) && condition)
            {
                if (phasesSequence.Contains(phase))
                {
                    CompleteCurrentPhaseInSequence();
                }
                else
                {
                    CompletePhase(phase);
                }
            }
        }

        #endregion
    }

}