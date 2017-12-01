using System.Collections.Generic;
using Antura.Core;
using UnityEngine;

namespace Antura.Profile
{
    /// <summary>
    /// Defines different phases for the First Contact.
    /// @note: these are not in sequence, the sequence is defined by the sequence list!
    /// </summary>
    public enum FirstContactPhase
    {
        Intro,

        Reward_FirstBig,

        Map_Play,
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

        Finished
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
        private FirstContactPhase currentPhase = FirstContactPhase.Intro;
        private List<FirstContactPhase> phasesSequence;

        // Debug
        public static bool VERBOSE = true;

        public static bool DISABLE_FIRST_CONTACT = false;

        private static bool SIMULATE_FIRST_CONTACT = false;
        private FirstContactPhase SIMULATE_FIRST_CONTACT_PHASE = FirstContactPhase.Reward_FirstBig;

        private static bool FORCE_FIRST_CONTACT = false;
        private FirstContactPhase FORCED_FIRST_CONTACT_PHASE = FirstContactPhase.Map_GoToBook;

        public FirstContactManager()
        {
            if (!Application.isEditor) {
                // Force debug options to FALSE if we're not in the editor
                SIMULATE_FIRST_CONTACT = false;
                FORCE_FIRST_CONTACT = false;
            }

            // Define the first contact sequence
            phasesSequence = new List<FirstContactPhase>
            {
                FirstContactPhase.Reward_FirstBig,

                FirstContactPhase.AnturaSpace_TouchAntura,
                FirstContactPhase.AnturaSpace_Customization,
                FirstContactPhase.AnturaSpace_Exit,

                FirstContactPhase.Map_Play,
                FirstContactPhase.Map_GoToAnturaSpace,

                FirstContactPhase.AnturaSpace_Shop,
                FirstContactPhase.AnturaSpace_Photo,

                FirstContactPhase.Finished
            };

        }

        public void InitialiseForCurrentPlayer()
        {
            if (FORCE_FIRST_CONTACT) {
                ForceAtPhase(FORCED_FIRST_CONTACT_PHASE);
            }
        }

        #region Checks

        public bool IsNotCompleted()
        {
            if (DISABLE_FIRST_CONTACT) { return false; }
            return CurrentPhase < FirstContactPhase.Finished || SIMULATE_FIRST_CONTACT;
        }

        public bool HasPassedPhase(FirstContactPhase _phase)
        {
            if (!phasesSequence.Contains(_phase)) { return true; } // not in the sequence, hence passed
            return phasesSequence.IndexOf(_phase) < phasesSequence.IndexOf(CurrentPhase);
        }

        public bool IsInPhase(FirstContactPhase _phase)
        {
            return CurrentPhase == _phase;
        }

        public FirstContactPhase CurrentPhase
        {
            get {
                if (SIMULATE_FIRST_CONTACT) {
                    return SIMULATE_FIRST_CONTACT_PHASE;
                }
                return currentPhase;
            }
        }

        #endregion

        #region Setters

        public void CompleteCurrentPhase()
        {
            // Advance in the sequence
            if (VERBOSE) { Debug.Log("FirstContact - phase " + currentPhase + " completed!"); }
            currentPhase = phasesSequence[phasesSequence.IndexOf(currentPhase) + 1];
            if (VERBOSE) { Debug.Log("FirstContact - phase " + currentPhase + " starting..."); }
        }

        public void PassPhase(FirstContactPhase passedPhase)
        {
            currentPhase = passedPhase + 1;
            AppManager.I.Player.Save();

            if (VERBOSE) Debug.Log("FirstContact - phase " + passedPhase + " completed!");
        }

        public void ForceAtPhase(FirstContactPhase forcedPhase)
        {
            currentPhase = forcedPhase;
            AppManager.I.Player.Save();

            if (VERBOSE) Debug.Log("FirstContact - FORCING phase " + forcedPhase);
        }

        public void ForceToCompleted()
        {
            ForceAtPhase(FirstContactPhase.Finished);
        }

        public void ForceToStart()
        {
            ForceAtPhase(FirstContactPhase.Intro);
            AppManager.I.Player.ResetPlayerProfileCompletion();
        }

        #endregion


        /// <summary>
        /// Filter the navigation from a scene to the next based on the first contact requirements
        /// </summary>
        public AppScene FilterNavigation(AppScene fromScene, AppScene toScene, out bool keepPrevAsBackable)
        {
            keepPrevAsBackable = false;
            if (!IsNotCompleted()) return toScene;

            // Check whether this transition is completing a phase
            TransitionCompletePhaseOn(FirstContactPhase.Intro, fromScene == AppScene.Intro);
            TransitionCompletePhaseOn(FirstContactPhase.AnturaSpace_Exit, fromScene == AppScene.AnturaSpace);
            TransitionCompletePhaseOn(FirstContactPhase.Map_Play, fromScene == AppScene.PlaySessionResult);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToAnturaSpace, fromScene == AppScene.Map && toScene == AppScene.AnturaSpace);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToBook, fromScene == AppScene.Map && toScene == AppScene.Book);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToMinigames, fromScene == AppScene.Map && toScene == AppScene.Book);
            TransitionCompletePhaseOn(FirstContactPhase.Map_GoToProfile, fromScene == AppScene.Map && toScene == AppScene.Book);

            // Check whether this transition needs to be filtered
            FilterTransitionOn(FirstContactPhase.Intro, fromScene == AppScene.Home && toScene == AppScene.Map, ref toScene, AppScene.Intro);
            FilterTransitionOn(FirstContactPhase.Reward_FirstBig, fromScene == AppScene.Intro, ref toScene, AppScene.Rewards);
            FilterTransitionOn(FirstContactPhase.AnturaSpace_TouchAntura, fromScene == AppScene.Rewards, ref toScene, AppScene.AnturaSpace);

            return toScene;
        }

        private void FilterTransitionOn(FirstContactPhase phase, bool condition, ref AppScene toScene, AppScene newScene)
        {
            if (CurrentPhase == phase && condition) { toScene = newScene; }
        }

        private void TransitionCompletePhaseOn(FirstContactPhase phase, bool condition)
        {
            if (CurrentPhase == phase && condition) { CompleteCurrentPhase(); }
        }

    }

}