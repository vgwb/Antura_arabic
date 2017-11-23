
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

    // What do we need for the first contact
    // - linear progression VS phases unlocked
    // - phases are SEPARATED, like a FSM, each scene will check for the current phase and register to those it is concerned with!
    // - the flow of the first contact is instead defined by a list of phases in order. Each phase will start when the previous is finished.
    //   - NOTE: this makes the flow LINEAR!  

    // - allow some parts to be unlocked with LBs
    // - move the player to AnturaSpace when you unlock the first reward
    // - tutorials also unlock
    // - reward 1 tutorial: 
    // - anturaspace 3 tutorials: customization, shop, photo
    // - map 2 tutorials: map, gotoAnturaSpace, gotoBook, gotoProfile, gotoMinigames
    // - map triggers on specific LB/PS to start the new 'phase'

    // TODO: define triggers that enable or not the tutorial!!!
        // - example: trigger after a specific PS (or when we increase MaxPS)

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
        private FirstContactPhase currentPhase = FirstContactPhase.Reward_FirstBig;
        private List<FirstContactPhase> phasesSequence;

        // Debug
        private static bool SIMULATE_FIRST_CONTACT = false;
        private FirstContactPhase SIMULATE_FIRST_CONTACT_PHASE = FirstContactPhase.Reward_FirstBig;

        private static bool FORCE_FIRST_CONTACT = true;
        private FirstContactPhase FORCED_FIRST_CONTACT_PHASE = FirstContactPhase.AnturaSpace_Customization;

        #region Checks

        public bool IsInsideFirstContact()
        {
            return CurrentPhase < FirstContactPhase.Finished || SIMULATE_FIRST_CONTACT;
        }

        public bool HasPassedPhase(FirstContactPhase _phase)
        {
            return CurrentPhase < _phase;
        }

        public bool IsInPhase(FirstContactPhase _phase)
        {
            return CurrentPhase == _phase;
        }

        public FirstContactPhase CurrentPhase
        {
            get
            {
                if (SIMULATE_FIRST_CONTACT) return SIMULATE_FIRST_CONTACT_PHASE;
                return currentPhase;
            }
        }

        #endregion

        #region Setters

        public void CompleteCurrentPhase()
        {
            // Advance in the sequence
            Debug.Log("First Contact phase " + currentPhase + " completed!");
            currentPhase = phasesSequence[phasesSequence.IndexOf(currentPhase) + 1];
            Debug.Log("First Contact phase " + currentPhase + " starting...");
        }

        public void PassPhase(FirstContactPhase passedPhase)
        {
            currentPhase = passedPhase + 1;
            AppManager.I.Player.Save();

            Debug.Log("First Contact phase " + passedPhase + " completed!");
        }

        public void ForceAtPhase(FirstContactPhase forcedPhase)
        {
            currentPhase = forcedPhase;
            AppManager.I.Player.Save();

            Debug.Log("FORCING First Contact phase " + forcedPhase);
        }

        #endregion

        public struct SceneTransition
        {
            public AppScene fromScene;
            public AppScene toScene;
            public bool keepAsBackable;

            public SceneTransition(AppScene fromScene, AppScene toScene, bool keepAsBackable = false)
            {
                this.fromScene = fromScene;
                this.toScene = toScene;
                this.keepAsBackable = keepAsBackable;
            }


            public bool Equals(SceneTransition other)
            {
                return fromScene == other.fromScene && toScene == other.toScene;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is SceneTransition && Equals((SceneTransition) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((int) fromScene * 397) ^ (int) toScene;
                }
            }

        }

        private Dictionary<SceneTransition, AppScene> filteredTransitionsMap = new Dictionary<SceneTransition, AppScene>();

        public FirstContactManager()
        {
            if (!Application.isEditor)
            {
                // Force debug options to FALSE if we're not in the editor
                SIMULATE_FIRST_CONTACT = false; 
                FORCE_FIRST_CONTACT = false;
            }

            phasesSequence = new List<FirstContactPhase>
            {
                FirstContactPhase.Reward_FirstBig,

                FirstContactPhase.AnturaSpace_TouchAntura,
                FirstContactPhase.AnturaSpace_Customization,
                FirstContactPhase.AnturaSpace_Exit,

                FirstContactPhase.Map_Play,
                FirstContactPhase.Map_GoToAnturaSpace,

                // TODO: the scene should handle MULTIPLE tutorials in sequence, disjointed (check after each tutorial ends!)
                FirstContactPhase.AnturaSpace_Shop,
                FirstContactPhase.AnturaSpace_Photo,

            };

            //SetupFilteredTransitions();
        }

        public void InitialiseForCurrentPlayer()
        {
            if (FORCE_FIRST_CONTACT)
            {
                ForceAtPhase(FORCED_FIRST_CONTACT_PHASE);
            }
        }

        //private void SetupFilteredTransitions()
        //{
        //}

        /// <summary>
        /// Filter the navigation from a scene to the next based on the first contact requirements
        /// </summary>
        public AppScene FilterNavigation(AppScene fromScene, AppScene toScene, out bool keepPrevAsBackable)
        {
            keepPrevAsBackable = false;
            if (!IsInsideFirstContact()) return toScene;

            // Check whether this transition is completing a tutorial phase
            if (fromScene == AppScene.Intro && CurrentPhase == FirstContactPhase.Intro) CompleteCurrentPhase();

            // TODO: Remove the map!
            filteredTransitionsMap = new Dictionary<SceneTransition, AppScene>();
            switch (CurrentPhase)
            {
                // Intro starts
                // Home->Map becomes Home->Intro
                case FirstContactPhase.Intro:
                    filteredTransitionsMap.Add(new SceneTransition(AppScene.Home, AppScene.Map), AppScene.Intro);
                    break;

                // BigReward awaiting
                // Home->Map becomes Home->Intro
                case FirstContactPhase.Reward_FirstBig:
                    filteredTransitionsMap.Add(new SceneTransition(AppScene.Intro, AppScene.Map), AppScene.Rewards);
                    break;

                // The BigReward ends. 
                // We go to AnturaSpace directly
                case FirstContactPhase.AnturaSpace_TouchAntura:
                    filteredTransitionsMap.Add(new SceneTransition(AppScene.Rewards, AppScene.Map), AppScene.AnturaSpace);
                    break;

                    /*
                // Map to Rewards directly instead of AnturaSpace when you finish the first map step (TODO)
                case FirstContactPhase.Map_GoToAnturaSpace:
                    filteredTransitionsMap.Add(new SceneTransition(AppScene.Map, AppScene.AnturaSpace, true), AppScene.Rewards);
                    // TODO: We must force the prev scene stack to hold the Map <-> AnturaSpace transition (should be in, test it!)
                    break;*/

                    /*
                // The Intro ends
                //Rewards to AnturaSpace directly when you earn the first reward
                // Rewards->Map becomes Rewards->Intro
                case FirstContactPhase.AnturaSpace_Customization:
                    filteredTransitionsMap.Add(new SceneTransition(AppScene.Intro, AppScene.Map), AppScene.AnturaSpace);
                    break;
                    */
            }

            // Handle the transition
            var currentTransition = new SceneTransition(fromScene, toScene);
            foreach (var filteredTransition in filteredTransitionsMap.Keys)
            {
                if (filteredTransition.Equals(currentTransition))
                {
                    keepPrevAsBackable = filteredTransition.keepAsBackable;
                    return filteredTransitionsMap[filteredTransition];
                }
            }

            return toScene;
        }

    }

}