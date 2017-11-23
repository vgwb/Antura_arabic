
using System.Collections.Generic;
using Antura.Core;

namespace Antura.Profile
{
    /// <summary>
    /// Manages the flow of First Contact (i.e. the tutorial) throughout the application
    /// </summary>
    public class FirstContactManager
    {
        public static FirstContactManager I
        {
            get { return AppManager.I.FirstContactManager; }
        }

        // TODO: FirstContactFlow that changes the NavigationManger based on FirstContact requirements!

        /// <summary>
        /// Defines the different phases of the First Contact
        /// The player will traverse these in sequence
        /// </summary>
        /// // TODO: maybe define them all, also in the first Reward and so on?
        public enum Phase
        {
            NewPlayer,
            Map_FirstEncounter,
            Map_SecondEncounter,
            Finished
        }

        // State
        private Phase phase = Phase.NewPlayer;

        // Parameters
        //[Header("Debug")]
        private bool SimulateFirstContact;

        #region Checks

        public bool IsInFirstContact()
        {
            return phase < Phase.Finished || SimulateFirstContact;
        }

        public bool HasPassedPhase(Phase _phase)
        {
            return phase < _phase;
        }

        public bool IsInPhase(Phase _phase)
        {
            return phase == _phase;
        }

        // TODO: Check PlayerProfile.IsInFirstContact()

        #endregion

        public struct SceneTransition
        {
            public AppScene fromScene;
            public AppScene toScene;

            public SceneTransition(AppScene fromScene, AppScene toScene)
            {
                this.fromScene = fromScene;
                this.toScene = toScene;
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
            // Setup filtered transitions

            // Home to Intro instead of map
            filteredTransitionsMap.Add(new SceneTransition(AppScene.Home,AppScene.Map), AppScene.Intro);
           
            // Rewards to AnturaSpace directly when you earn the first reward
            filteredTransitionsMap.Add(new SceneTransition(AppScene.Rewards, AppScene.Map), AppScene.AnturaSpace);

            // Map to Rewards directly instead of AnturaSpace.
            filteredTransitionsMap.Add(new SceneTransition(AppScene.Map, AppScene.AnturaSpace), AppScene.Rewards);
            // TODO: We must force the prev scene stack to hold the Map <-> AnturaSpace transition
            //    UpdatePrevSceneStack(AppScene.AnturaSpace);

        }

        /// <summary>
        /// Filter the navigation from a scene to the next based on the first contact requirements
        /// </summary>
        public AppScene FilterNavigation(AppScene fromScene, AppScene toScene)
        {
            if (!IsInFirstContact()) return toScene;

            var currentTransition = new SceneTransition(fromScene, toScene);
            foreach (var filteredTransition in filteredTransitionsMap.Keys)
            {
                if (filteredTransition.Equals(currentTransition))
                {
                    return filteredTransitionsMap[filteredTransition];
                }
            }

            return toScene;
        }
    }
}